using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Core.Messaging.EventBus;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;
using LunaticPanel.Engine.Keys.System;
using LunaticPanel.Engine.Web.Boostrap.Plugin;
using MudBlazor;
using System;
using System.Net;
using System.Text.Json;

namespace LunaticPanel.Engine.Web.Services.UpdateService;

internal class PanelUpdateService : IPanelUpdateService
{

    public Version? Downloaded { get; private set; }

    public Version? Install { get; private set; }

    public bool AwaitingForPluginCompatibilityUpdate { get; private set; }

    public bool BlockedByPlugin { get; private set; }

    public Version? Installing { get; private set; }
    public Version? Staged { get; private set; }
    public Dictionary<string, BootstrapPluginManifest>? PluginUpgradeDownloaded { get; private set; }
    public List<string> PreInstalled { get; private set; } = new();
    private readonly string _updateEndpoint = "https://update.lunaticpanel.com/release/";
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ISafeFileWriter _safeFileWriter;
    private readonly ICircuitRegistry _circuitRegistry;
    private readonly IEventBus _eventBus;
    private readonly ICrazyReport<PanelUpdateService> _crazyReport;
    private readonly Version _currentPanelVersion;
    private readonly string _upgradeRootFolder;
    private readonly string _upgradeDownloadFolder;
    private readonly string _upgradeApplyFolder;
    private readonly string _upgradePluginUpgradeFolder;
    private readonly string _pluginStageLocationRootFolder;
    private readonly string _pluginApplyLocationFolder;
    private readonly string _pluginRollbackLocationFolder;
    private readonly string _pluginInstalledLocationFolder;
    private readonly string _pluginDeleteLocationFolder;
    private readonly string _binPreinstalledPlugins;
    private readonly string _upgradeBackup;
    private readonly bool _autoRestart = true;
    private readonly bool _previewEnabled = false;
    private string EnsureCreated(string path, bool clearExist = false)
    {
        if (string.IsNullOrWhiteSpace(path)) return path;
        if (clearExist && Directory.Exists(path))
            Directory.Delete(path, true);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }
    public PanelUpdateService(
        IHttpClientFactory httpClientFactory,
        ISafeFileWriter safeFileWriter, ICircuitRegistry circuitRegistry,
        IEventBus eventBus,
        ICrazyReport<PanelUpdateService> crazyReport)
    {
        _httpClientFactory = httpClientFactory;
        _safeFileWriter = safeFileWriter;
        _circuitRegistry = circuitRegistry;
        _eventBus = eventBus;
        _crazyReport = crazyReport;
        _currentPanelVersion = typeof(IPlugin).Assembly.GetName().Version!;
        _upgradeRootFolder = Path.Combine($"{Path.DirectorySeparatorChar}", "var", "lib", "lunaticpanel", ".upgrade");
        _upgradeDownloadFolder = Path.Combine(_upgradeRootFolder, "download");
        _upgradeApplyFolder = Path.Combine(_upgradeRootFolder, "apply");
        _upgradePluginUpgradeFolder = Path.Combine(_upgradeRootFolder, "plugins");
        _upgradeBackup = Path.Combine($"{Path.DirectorySeparatorChar}", "var", "lib", "lunaticpanel", ".upgrade", "backups");
        CreateRequiredFolders();
        _pluginStageLocationRootFolder = Path.Combine($"{Path.DirectorySeparatorChar}", "var", "lib", "lunaticpanel", ".plugins");
        _pluginApplyLocationFolder = Path.Combine(_pluginStageLocationRootFolder, "apply");
        _pluginDeleteLocationFolder = Path.Combine(_pluginStageLocationRootFolder, "delete");
        _pluginRollbackLocationFolder = Path.Combine(_pluginStageLocationRootFolder, "rollbacks");
        _pluginInstalledLocationFolder = Path.Combine(_pluginStageLocationRootFolder, "installed");
        _binPreinstalledPlugins = Path.Combine($"{Path.DirectorySeparatorChar}", "srv", "lunaticpanel", "plugins_preinstalled");
        if (bool.TryParse(Environment.GetEnvironmentVariable("LP_UPGRADE_AUTO_RESTART"), out var autoRestart))
            _autoRestart = autoRestart;

        if (bool.TryParse(Environment.GetEnvironmentVariable("LP_OPT_OUT_PREVIEW"), out var previewOptOut))
            _previewEnabled = !previewOptOut;

        SafeToUpgrade();
    }

    private void CreateRequiredFolders()
    {
        EnsureCreated(_upgradeRootFolder);
        EnsureCreated(_upgradeDownloadFolder);
        EnsureCreated(_upgradeApplyFolder);
        EnsureCreated(_upgradePluginUpgradeFolder);
        EnsureCreated(_upgradeBackup);
    }
    public async Task CheckPluginsAsync(CancellationToken ct = default)
    {
        if (Installing != default || Staged != default)
            return;

        SafeToUpgrade();
        if (BlockedByPlugin) return;

        string lastDownloaded = Path.Combine(_upgradeDownloadFolder, "cache.txt");
        if (!File.Exists(lastDownloaded)) return;
        PluginUpgradeDownloaded = default;
        AwaitingForPluginCompatibilityUpdate = true;
        Version versionCache = new Version(File.ReadAllText(lastDownloaded));
        if (versionCache <= _currentPanelVersion) return;
        string[] installed = Directory.GetFiles(_pluginInstalledLocationFolder, "*.lpkg", SearchOption.TopDirectoryOnly);
        string[] plugins = Directory.GetFiles(_upgradePluginUpgradeFolder, "*.lpkg", SearchOption.TopDirectoryOnly);
        Dictionary<string, BootstrapPluginManifest> validForNextUpgrade = new();


        foreach (var item in installed)
        {
            var manifest = await ArchiveUtilityExt.ReadJson<BootstrapPluginManifest>(item, "manifest.json", ArchiveType.LunaPackage);
            if (manifest == default) continue;
            Version pPanelVersion = new(manifest.PanelVersion);
            if (pPanelVersion <= versionCache && pPanelVersion.Major == versionCache.Major && !validForNextUpgrade.Any(p => p.Value.Id == manifest.Id))
                File.Copy(item, Path.Combine(_upgradePluginUpgradeFolder, Path.GetFileName(item)));
        }


        plugins = Directory.GetFiles(_upgradePluginUpgradeFolder, "*.lpkg", SearchOption.TopDirectoryOnly);

        foreach (var item in plugins)
        {
            var manifest = await ArchiveUtilityExt.ReadJson<BootstrapPluginManifest>(item, "manifest.json", ArchiveType.LunaPackage);
            if (manifest == default)
            {
                _crazyReport.ReportWarning($"'{item}' failed to extract manifest.");
                File.Delete(item);
                _crazyReport.ReportWarning($"'{item}' Deleted.");
                continue;
            }
            if (validForNextUpgrade.Any(p => p.Value.Id == manifest.Id))
            {
                _crazyReport.ReportWarning($"'{item}' failed to extract manifest.");
                File.Delete(item);
                _crazyReport.ReportWarning($"'{item}' Deleted.");
                return;
            }
            Version pPanelVersion = new(manifest.PanelVersion);
            if (_currentPanelVersion.Major != versionCache.Major)
            {
                _crazyReport.ReportWarning($"Panel version for plugin {manifest.Id} found '{item}' is not the same as the nextPlanned upgrade major {versionCache.Major}.");
                File.Delete(item);
                _crazyReport.ReportWarning($"'{item}' Deleted.");
                continue;
            }
            if (versionCache > pPanelVersion)
            {
                _crazyReport.ReportWarning($"Panel version for plugin {manifest.Id} found '{item}' was compiled using a more recent panel version ({pPanelVersion} > {versionCache}).");
                File.Delete(item);
                _crazyReport.ReportWarning($"'{item}' Deleted.");
                continue;
            }
            var existing = validForNextUpgrade.SingleOrDefault(p => p.Value.Id == manifest.Id);
            if (validForNextUpgrade.Any(p => p.Value.Id == manifest.Id))
            {
                _crazyReport.ReportError($"Skipping {manifest.Id} a compatible version of the same plugin already exist.");
                continue;

            }
            validForNextUpgrade[item] = manifest;
        }
        PreInstalled.Clear();
        if (Directory.Exists(_binPreinstalledPlugins))
        {
            string[] preinstalled = Directory.GetFiles(_binPreinstalledPlugins, "*.lpkg", SearchOption.TopDirectoryOnly);

            foreach (var item in preinstalled)
            {
                var manifest = await ArchiveUtilityExt.ReadJson<BootstrapPluginManifest>(item, "manifest.json", ArchiveType.LunaPackage);
                if (manifest == default) throw new Exception($"Manifest failed to read from lunapackage file. '{item}'");
                PreInstalled.Add(manifest.Id);
            }

        }

        bool nextFlag = false;
        foreach (var item in installed)
        {
            var manifest = await ArchiveUtilityExt.ReadJson<BootstrapPluginManifest>(item, "manifest.json", ArchiveType.LunaPackage);
            if (manifest == default) throw new Exception($"Manifest failed to read from lunapackage file. '{item}'");


            if (!validForNextUpgrade.Any(p => p.Value.Id == manifest.Id))
            {
                _crazyReport.ReportWarning($"Still awaiting valid plugin upgrade package built on panel version same major <= {versionCache}).");
                nextFlag = true;
            }
        }
        PluginUpgradeDownloaded = validForNextUpgrade;
        AwaitingForPluginCompatibilityUpdate = nextFlag;
    }
    private void SafeToUpgrade()
    {
        string[] pendingUpdate = Directory.GetFiles(_pluginApplyLocationFolder, "*.lpkg", SearchOption.TopDirectoryOnly);
        string[] pendingDelete = Directory.GetFiles(_pluginDeleteLocationFolder, "*.lpkg", SearchOption.TopDirectoryOnly);
        if (pendingUpdate.Length > 0 || pendingDelete.Length > 0)
        {
            _crazyReport.ReportWarning("Panel Update is blocked by pending plugin install/update or delete... you must restart the panel.");
            BlockedByPlugin = true;
        }
        else
            BlockedByPlugin = false;
    }
    public async Task DownloadUpdateAsync(CancellationToken ct = default)
    {
        if (Installing != default || Staged != default)
            return;
        SafeToUpgrade();
        if (BlockedByPlugin) return;
        // Create the short-lived client from the injected factory
        using var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_updateEndpoint);
        var version = await GetLatestVersion(client, ct);
        if (version <= _currentPanelVersion) return;
        string lastDownloaded = Path.Combine(_upgradeDownloadFolder, "cache.txt");
        // _previewEnabled
        // Download Panel
        await DownloadFile(client, $"lunaticpanel_{version}.tar.gz", Path.Combine(_upgradeDownloadFolder, "lunaticpanel_latest.tar.gz"), ct);
        // Download Plugins
        string officialPanelPlugins = Path.Combine(_upgradeDownloadFolder, "lunaticpanel_plugins_latest.tar.gz");
        await DownloadFile(client, $"lunaticpanel_plugins_{version}.tar.gz", officialPanelPlugins, ct);
        ResetFolder(_upgradePluginUpgradeFolder);
        // Send all plugins into plugin upgrade staging folder.
        await ArchiveUtilityExt.ExtractTo(officialPanelPlugins, _upgradePluginUpgradeFolder, ArchiveType.TarGz);
        await _safeFileWriter.WriteThenCopyFileAsync(lastDownloaded, version!.ToString(3));
        Downloaded = version;
        await CheckPluginsAsync(ct);
    }

    public async Task InstallUpdateAsync(CancellationToken ct = default)
    {
        if (Installing != default || Downloaded == default)
            return;
        Installing = Downloaded;
        int backupFileCount = Directory.GetFiles(_upgradeBackup, "*.tar.gz").Count();
        int nextCount = backupFileCount + 1;
        string backupPanelFile = Path.Combine(_upgradeBackup, $"panel-{nextCount}.tar.gz");
        string backupPluginFile = Path.Combine(_upgradeBackup, $"plugins-{nextCount}.tar.gz");
        string upgradeVersionCache = Path.Combine(_upgradeApplyFolder, $"version.txt");
        string binPanelPath = Path.Combine($"{Path.DirectorySeparatorChar}", "srv", "lunaticpanel");
        string stagingPluginInstalled = Path.Combine(_pluginStageLocationRootFolder, "installed");
        var downloadedPanel = Path.Combine(_upgradeDownloadFolder, "lunaticpanel_latest.tar.gz");
        var applyPanelFileLocation = Path.Combine(_upgradeApplyFolder, "lunaticpanel_latest.tar.gz");
        var applyUserInstalledolderLocation = EnsureCreated(Path.Combine(_upgradeApplyFolder, "plugin_user_installed"), true);
        var applyPreInstalledFolderLocation = EnsureCreated(Path.Combine(_upgradeApplyFolder, "plugin_preinstalled"), true);
        try
        {
            _crazyReport.ReportInfo($"Panel version {_currentPanelVersion.ToString(3)} backed up ('{backupPanelFile}')");
            _crazyReport.ReportInfo($"Panel installed plugins for panel {_currentPanelVersion.ToString(3)} backed up ('{backupPluginFile}')");

            SafeToUpgrade();
            if (BlockedByPlugin)
            {
                _crazyReport.ReportWarning("Panel cannot install upgrade because it is still blocked by plugin pending delete, install or update.. you must restart the panel before upgrading.");
                return;
            }
            await CheckPluginsAsync(ct);
            if (AwaitingForPluginCompatibilityUpdate)
            {
                _crazyReport.ReportWarning("Panel cannot install upgrade because it is still waiting for the plugin's compatibility upgrade to be downloaded.");
                return;
            }
            if ((PluginUpgradeDownloaded == default && PreInstalled.Count > 0) || (PluginUpgradeDownloaded != default && PreInstalled.Count > PluginUpgradeDownloaded.Count))
            {
                _crazyReport.ReportError("Panel cannot upgrade as there is a mismatch between preinstalled and upgraded package folder.");
                return;
            }
            File.Move(downloadedPanel, applyPanelFileLocation, true);
            if (PluginUpgradeDownloaded != default)
                foreach (var item in PluginUpgradeDownloaded)
                    if (PreInstalled.Contains(item.Value.Id))
                        File.Move(item.Key, Path.Combine(applyPreInstalledFolderLocation, $"{item.Value.Id}.{item.Value.Version}.lpkg"), true);
                    else
                        File.Move(item.Key, Path.Combine(applyUserInstalledolderLocation, $"{item.Value.Id}.{item.Value.Version}.lpkg"), true);

            // Clear Working Files
            ResetFolder(_upgradePluginUpgradeFolder);
            ResetFolder(_upgradeDownloadFolder);

            await _safeFileWriter.WriteThenCopyFileAsync(upgradeVersionCache, Installing.ToString(3));
            await _safeFileWriter.WriteThenCopyFileAsync(Path.Combine(binPanelPath, "access.test"), Installing.ToString(3));
            await _safeFileWriter.WriteThenCopyFileAsync(Path.Combine(_upgradeRootFolder, "access.test"), Installing.ToString(3));
            await _safeFileWriter.WriteThenCopyFileAsync(Path.Combine(_pluginStageLocationRootFolder, "access.test"), Installing.ToString(3));

            Staged = Installing;
            _crazyReport.ReportInfo($"Auto Restart ? {_autoRestart} ");
            _crazyReport.ReportInfo($"Active Circuits ? {_circuitRegistry.GetActiveCircuits().Count} ");

            if (_autoRestart && _circuitRegistry.GetActiveCircuits().Count <= 1)
            {
                _crazyReport.ReportInfo("Auto Restart is active and no one is using the panel, therefore let's restart.");
                await _eventBus.PublishDatalessAsync(PanelKeys.Events.PanelShutdown);
            }
            else if (_autoRestart && _circuitRegistry.GetActiveCircuits().Count > 1)
                _crazyReport.Report("Auto Restart is enabled but active circuits always prevent auto restart to avoid losing data.");

            if (!_autoRestart)
                _crazyReport.Report("Auto Restart is disabled you must restart the panel manually.");

        }
        catch (System.Exception ex)
        {
            _crazyReport.ReportErrorException(ex.Message, ex);


        }
        finally
        {
            Installing = default;
        }


        // _binPreinstalledPlugins

    }

    private void ResetFolder(string path)
    {
        if (Directory.Exists(path))
            Directory.Delete(path, true);
        EnsureCreated(path);
    }
    public async Task<Version> GetLatestVersion(CancellationToken ct = default)
    {
        using (var client = _httpClientFactory.CreateClient())
        {
            client.BaseAddress = new Uri(_updateEndpoint);
            var version = await GetLatestVersion(client, ct);
            if (version == default)
                throw new Exception("Couldn't fetch remote version.");
            if (version >= _currentPanelVersion)
                return version;
            return _currentPanelVersion;
        }

    }

    private async Task<Version?> GetLatestVersion(HttpClient client, CancellationToken ct = default)
    {
        var responseStable = await client.GetAsync("stable.txt");
        responseStable.EnsureSuccessStatusCode();
        string versionStableRaw = await responseStable.Content.ReadAsStringAsync();
        _crazyReport.Report($"Remote Panel Version is {versionStableRaw}");
        // TODO: Add Support for Preview Versioning.
        // if (_previewEnabled)
        // {
        //     var responsePreview = await client.GetAsync("stable.txt");
        //     if (responsePreview.IsSuccessStatusCode)
        //     {
        //         string versionPreviewRaw = await responsePreview.Content.ReadAsStringAsync();


        //     }
        // }
        return new Version(versionStableRaw);
    }
    public async Task DownloadFile(HttpClient client, string remoteFile, string targetLocation, CancellationToken ct = default)
    {
        string destinationPathDownload = Path.GetTempFileName();
        using var response = await client.GetAsync(remoteFile, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        using (var downloadStream = await response.Content.ReadAsStreamAsync())
        {
            using (var fileStream = new FileStream(destinationPathDownload, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
            {
                await downloadStream.CopyToAsync(fileStream);
            }
        }
        File.Move(destinationPathDownload, targetLocation, true); // Final Move.
    }

}
