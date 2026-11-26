using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Core.Utils.Abstraction.IO;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Engine.Bootstraper;
using LunaticPanel.Engine.Infrastructure.Interfaces;
using LunaticPanel.Engine.Keys.System;
using LunaticPanel.Engine.Plugin;
using NuGet.Versioning;
using System.Reflection;

namespace LunaticPanel.Engine.Infrastructure.Implementations;

internal class PanelUpdateService : IPanelUpdateService
{

    public NuGetVersion? Downloaded { get; private set; }

    public NuGetVersion? Install { get; private set; }

    public bool AwaitingForPluginCompatibilityUpdate { get; private set; }

    public bool BlockedByPlugin { get; private set; }

    public NuGetVersion? Installing { get; private set; }
    public NuGetVersion? Staged { get; private set; }
    public Dictionary<string, PluginManifestEntity>? PluginUpgradeDownloaded { get; private set; }
    public List<string> PreInstalled { get; private set; } = new();
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAtomicFileSystem _atomicFileSystem;
    private readonly ICircuitRegistry _circuitRegistry;
    private readonly IUpdateDiscoveryLocator _updateDiscoveryLocator;
    private readonly IUpdateDiscovery _updateDiscovery;
    private readonly IPluginDiscoveryLocator _pluginDiscoveryLocator;
    private readonly IEventBus _eventBus;
    private readonly ICrazyReport<PanelUpdateService> _crazyReport;
    private readonly NuGetVersion _currentPanelVersion;
    private readonly bool _autoRestart = true;
    private readonly bool _previewEnabled = true;
    private readonly Uri _baseRemoteUrl;
    private readonly Uri _previewCheckUrl;
    private readonly Uri _stableCheckUrl;
    private readonly string _downloadLocation;
    private readonly TimeSpan _fendOffSetting = new(1, 0, 0, 0);
    public PanelUpdateService(
        IHttpClientFactory httpClientFactory,
        IAtomicFileSystem AtomicFileSystem,
        ICircuitRegistry circuitRegistry,
        IUpdateDiscoveryLocator updateDiscoveryLocator,
        IUpdateDiscovery updateDiscovery,
        IPluginDiscoveryLocator pluginDiscoveryLocator,
        IEventBus eventBus,
        ICrazyReport<PanelUpdateService> crazyReport)
    {
        _httpClientFactory = httpClientFactory;
        _atomicFileSystem = AtomicFileSystem;
        _circuitRegistry = circuitRegistry;
        _updateDiscoveryLocator = updateDiscoveryLocator;
        _updateDiscovery = updateDiscovery;

        _pluginDiscoveryLocator = pluginDiscoveryLocator;
        _eventBus = eventBus;
        _crazyReport = crazyReport;
        _currentPanelVersion = NuGetVersion.Parse(typeof(IPlugin).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion);
        if (bool.TryParse(Environment.GetEnvironmentVariable("LP_UPGRADE_AUTO_RESTART"), out var autoRestart))
            _autoRestart = autoRestart;

        if (TimeSpan.TryParse(Environment.GetEnvironmentVariable("LP_UPDATE_FEND_OFF"), out var updateFendOff))
            _fendOffSetting = updateFendOff;

        if (bool.TryParse(Environment.GetEnvironmentVariable("LP_OPT_OUT_PREVIEW"), out var previewOptOut))
            _previewEnabled = !previewOptOut;
        _baseRemoteUrl = new Uri("https://update.lunaticpanel.com/release/");
        _previewCheckUrl = new Uri(_baseRemoteUrl, "preview.txt");
        _stableCheckUrl = new Uri(_baseRemoteUrl, "stable.txt");
        _downloadLocation = Pathf.CombineReady(_updateDiscoveryLocator.UpgradeRoot, "download");
        SafeToUpgrade();
    }

    public async Task CheckPluginsAsync(CancellationToken ct = default)
    {
        if (Installing != default || Staged != default)
        {
            _crazyReport.Report($"Installing Version {(Installing != default ? Installing : "NO")} Staged Folder {(Staged != default ? Staged : "NO")} Skip CheckPlugins.");

            return;
        }

        SafeToUpgrade();
        if (BlockedByPlugin) return;

        string lastDownloaded = Path.Combine(_downloadLocation, "cache.txt");
        if (!File.Exists(lastDownloaded))
        {
            _crazyReport.Report($"Cached Version Missing ('{lastDownloaded}').");
            return;
        }

        PluginUpgradeDownloaded = default;
        AwaitingForPluginCompatibilityUpdate = true;
        NuGetVersion versionCache = NuGetVersion.Parse(File.ReadAllText(lastDownloaded));
        if (versionCache <= _currentPanelVersion)
        {
            _crazyReport.Report($"Cached Version {versionCache} lower or equal to current {_currentPanelVersion} Skip CheckPlugins.");

            return;
        }
        string[] installed = Directory.GetFiles(_pluginDiscoveryLocator.PluginLifecycleInstalledLocation, "*.lpkg", SearchOption.TopDirectoryOnly);
        string[] plugins = Directory.GetFiles(_updateDiscoveryLocator.UpgradePluginCache, "*.lpkg", SearchOption.TopDirectoryOnly);
        Dictionary<string, PluginManifestEntity> validForNextUpgrade = new();
        foreach (var item in installed)
        {
            var manifest = await ArchiveUtilityExt.ReadJson<PluginManifestEntity>(item, "manifest.json", ArchiveType.LunaPackage);
            if (manifest == default) continue;
            NuGetVersion pPanelVersion = new(manifest.PanelVersion);
            if (pPanelVersion <= versionCache && pPanelVersion.Major == versionCache.Major && !validForNextUpgrade.Any(p => p.Value.Id == manifest.Id))
                File.Copy(item, Pathf.Combine(_updateDiscoveryLocator.UpgradePluginCache, Path.GetFileName(item)), true);
        }


        plugins = Directory.GetFiles(_updateDiscoveryLocator.UpgradePluginCache, "*.lpkg", SearchOption.TopDirectoryOnly);

        foreach (var item in plugins)
        {
            var manifest = await ArchiveUtilityExt.ReadJson<PluginManifestEntity>(item, "manifest.json", ArchiveType.LunaPackage);
            if (manifest == default)
            {
                _crazyReport.ReportWarning($"'{item}' failed to extract manifest.");
                File.Delete(item);
                _crazyReport.ReportWarning($"'{item}' Deleted.");
                continue;
            }
            if (validForNextUpgrade.Any(p => p.Value.Id == manifest.Id))
            {
                _crazyReport.ReportWarning($"'{item}' another valid version is already selected.");
                File.Delete(item);
                _crazyReport.ReportWarning($"'{item}' Deleted.");
                continue;
            }
            NuGetVersion pluginPanelVersion = new(manifest.PanelVersion);
            if (_currentPanelVersion.Major != versionCache.Major)
            {
                _crazyReport.ReportWarning($"Panel version for plugin {manifest.Id} found '{item}' is not the same as the nextPlanned upgrade major {versionCache.Major}.");
                File.Delete(item);
                _crazyReport.ReportWarning($"'{item}' Deleted.");
                continue;
            }
            if (pluginPanelVersion > versionCache)
            {
                _crazyReport.ReportWarning($"Panel version for plugin {manifest.Id} found '{item}' was compiled using a more recent panel version ({pluginPanelVersion} > {versionCache}).");
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
        if (Directory.Exists(_pluginDiscoveryLocator.PluginLifecyclePreInstalledLocation))
        {
            string[] preinstalled = Directory.GetFiles(_pluginDiscoveryLocator.PluginLifecyclePreInstalledLocation, "*.lpkg", SearchOption.TopDirectoryOnly);

            foreach (var item in preinstalled)
            {
                var manifest = await ArchiveUtilityExt.ReadJson<PluginManifestEntity>(item, "manifest.json", ArchiveType.LunaPackage);
                if (manifest == default) throw new Exception($"Manifest failed to read from lunapackage file. '{item}'");
                PreInstalled.Add(manifest.Id);
            }

        }

        bool nextFlag = false;
        foreach (var item in installed)
        {
            var manifest = await ArchiveUtilityExt.ReadJson<PluginManifestEntity>(item, "manifest.json", ArchiveType.LunaPackage);
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
        string[] pendingUpdate = Directory.GetFiles(_pluginDiscoveryLocator.PluginLifecycleApplyLocation, "*.lpkg", SearchOption.TopDirectoryOnly);
        string[] pendingDelete = Directory.GetFiles(_pluginDiscoveryLocator.PluginLifecycleDeleteLocation, "*.lpkg", SearchOption.TopDirectoryOnly);
        if (pendingUpdate.Length > 0 || pendingDelete.Length > 0)
        {
            _crazyReport.ReportWarning("Panel Update is blocked by pending plugin install/update or delete... you must restart the panel.");
            BlockedByPlugin = true;
        }
        else
        {
            BlockedByPlugin = false;
        }
    }

    public async Task DownloadUpdateAsync(CancellationToken ct = default)
    {
        SafeToUpgrade();
        if (BlockedByPlugin) return;
        // Create the short-lived client from the injected factory
        using var client = _httpClientFactory.CreateClient();
        var version = await GetLatestVersion(client, ct);
        if (version <= _currentPanelVersion)
        {
            _crazyReport.ReportWarning($"Latest Version {version} is lower or equal to current panel version {_currentPanelVersion}.");
            return;
        }
        _updateDiscovery.LoadVersionFile();
        if (_updateDiscovery.UpgradeTo != default && _updateDiscovery.UpgradeTo >= version)
        {
            _crazyReport.ReportWarning($"Update {_updateDiscovery.UpgradeTo} More Recent and Already Downloaded.");
            return;
        }

        string lastDownloaded = Path.Combine(_downloadLocation, "cache.txt");
        string officialPanel = Path.Combine(_downloadLocation, "lunaticpanel_latest.tar.gz");
        string officialPanelPlugins = Path.Combine(_downloadLocation, "lunaticpanel_plugins_latest.tar.gz");
        var panelUrl = new Uri(_baseRemoteUrl, $"lunaticpanel_{version}.tar.gz");
        var pluginUrl = new Uri(_baseRemoteUrl, $"lunaticpanel_plugins_{version}.tar.gz");
        await DownloadFile(client, panelUrl.ToString(), officialPanel, ct);
        await DownloadFile(client, pluginUrl.ToString(), officialPanelPlugins, ct);


        Pathf.ResetDir(_updateDiscoveryLocator.UpgradePluginCache);
        await ArchiveUtilityExt.ExtractTo(officialPanelPlugins, _updateDiscoveryLocator.UpgradePluginCache, ArchiveType.TarGz);
        await _atomicFileSystem.WriteAtomicAsync(lastDownloaded, version.ToString());
        Downloaded = version;
        await _eventBus.PublishDataAsync(PanelKeys.Events.ON_PANEL_UPDATE_DOWNLOADED, version.ToString());
        await CheckPluginsAsync(ct);
    }

    public async Task StageUpdateAsync(CancellationToken ct = default)
    {

        if (Installing != default || Downloaded == default)
        {
            _crazyReport.Report($"Installing Version {(Installing != default ? Installing : "NO")} Staged Folder {(Staged != default ? Staged : "NO")} Skip InstallUpdate.");
            return;
        }


        Installing = Downloaded;

        string upgradeVersionCache = Pathf.CombineReadyFile(_updateDiscoveryLocator.UpgradeReadyApply, "version.txt");
        string binPanelPath = _updateDiscoveryLocator.BinaryPathRoot;
        var downloadedPanel = Pathf.CombineReadyFile(_downloadLocation, "lunaticpanel_latest.tar.gz");
        var applyPanelFileLocation = Pathf.CombineReadyFile(_updateDiscoveryLocator.UpgradeReadyApply, "lunaticpanel_latest.tar.gz");
        var applyUserInstalledolderLocation = _updateDiscoveryLocator.UpgradePluginInstalled;
        var applyPreInstalledFolderLocation = _updateDiscoveryLocator.UpgradePluginPreinstalled;
        try
        {

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
            var releaseDate = await ArchiveUtilityExt.GetTimeOfTarGzFile(downloadedPanel);
            var fendOffExpiry = releaseDate.Add(_fendOffSetting);
            if (DateTime.UtcNow <= fendOffExpiry)
            {
                _crazyReport.ReportWarning("Panel Cannot Upgrade yet, still waiting for security fend off time to pass.");

            }

            await _updateDiscovery.ClearStageAsync();
            File.Move(downloadedPanel, applyPanelFileLocation, true);
            if (PluginUpgradeDownloaded != default)
                foreach (var item in PluginUpgradeDownloaded)
                {
                    var moveTo = Pathf.CombineReadyFile(applyPreInstalledFolderLocation, $"{item.Value.Id}.{item.Value.Version}.lpkg");
                    if (PreInstalled.Contains(item.Value.Id))
                        File.Move(item.Key, moveTo, true);
                    else
                        File.Move(item.Key, moveTo, true);
                }

            // Clear Working Files
            Pathf.ResetDir(_downloadLocation);
            await _atomicFileSystem.WriteAtomicAsync(upgradeVersionCache, Installing.ToString());
            await _atomicFileSystem.WriteAtomicAsync(Pathf.CombineReadyFile(binPanelPath, "access.test"), Installing.ToString());
            await _atomicFileSystem.WriteAtomicAsync(Pathf.CombineReadyFile(_updateDiscoveryLocator.UpgradeRoot, "access.test"), Installing.ToString());
            await _atomicFileSystem.WriteAtomicAsync(Pathf.CombineReadyFile(_pluginDiscoveryLocator.PluginLifecycleLocation, "access.test"), Installing.ToString());

            Staged = Installing;
            _crazyReport.ReportInfo($"Auto Restart ? {_autoRestart} ");
            _crazyReport.ReportInfo($"Active Circuits ? {_circuitRegistry.GetActiveCircuits().Count} ");
            bool restart = _autoRestart && _circuitRegistry.GetActiveCircuits().Count <= 1;

            if (_autoRestart && !restart)
                _crazyReport.Report("Auto Restart is enabled but active circuits always prevent auto restart to avoid losing data.");
            else if (!_autoRestart)
                _crazyReport.Report("Auto Restart is disabled you must restart the panel manually.");

            if (restart)
            {
                _crazyReport.ReportInfo("Auto Restart is active and no one is using the panel, therefore let's restart.");
                await _eventBus.PublishDatalessAsync(PanelKeys.Events.PANEL_SHUTDOWN);
            }
            else
            {
                await _eventBus.PublishDataAsync(PanelKeys.Events.ON_PANEL_UPDATE_STAGED, Installing.ToString());
            }

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


    private async Task<NuGetVersion> GetLatestVersion(HttpClient client, CancellationToken ct = default)
    {
        var responseStable = await client.GetAsync(_stableCheckUrl);
        responseStable.EnsureSuccessStatusCode();
        string versionStableRaw = await responseStable.Content.ReadAsStringAsync();
        versionStableRaw = versionStableRaw.ClearNewLines().ClearSpaces();
        var latest = NuGetVersion.Parse(versionStableRaw);
        _crazyReport.Report($"Remote Panel Version is {versionStableRaw}");
        if (_previewEnabled)
        {
            var responsePreview = await client.GetAsync(_previewCheckUrl);
            responseStable.EnsureSuccessStatusCode();
            string versionPreviewRaw = await responsePreview.Content.ReadAsStringAsync();
            var previewLatest = NuGetVersion.Parse(versionPreviewRaw);
            if (previewLatest > latest)
                latest = previewLatest;
        }
        if (_currentPanelVersion < latest)
            await _eventBus.PublishDataAsync(PanelKeys.Events.ON_PANEL_UPDATE_AVAILABLE, latest);
        return latest;
    }

    public async Task DownloadFile(HttpClient client, string remoteFile, string targetLocation, CancellationToken ct = default)
    {
        _crazyReport.Report($"Download '{remoteFile}' to '{targetLocation}'");
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

    public Task<NuGetVersion> GetLatestVersion(CancellationToken ct = default) => GetLatestVersion(_httpClientFactory.CreateClient(), ct);
}
