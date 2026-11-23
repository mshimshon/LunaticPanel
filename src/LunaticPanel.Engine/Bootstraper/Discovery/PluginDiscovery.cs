using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Engine.Plugin;
using Microsoft.Extensions.Configuration;
using MudBlazor;
using NuGet.Versioning;
using System;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace LunaticPanel.Engine.Bootstraper.Discovery;

public class PluginDiscovery : IPluginDiscovery, IDisposable
{
    internal readonly IPluginDiscoveryLocator _locator;
    private readonly IPluginStateController _pluginStateController;
    internal List<IPluginReference> _loadedPlugins = new();
    internal List<BootstrapStatusEntity> _bootstrapState = new();
    internal List<PluginManifestEntity> _preInstalled = new();
    internal List<string> _runtimeTargets = new();
    internal List<PluginManifestEntity> _toApplyUpdates = new();
    internal List<PluginManifestEntity> _installed = new();
    internal List<PluginManifestEntity> _delete = new();
    internal List<PluginManifestEntity> _rollbacks = new();
    private bool _disposedValue;
    internal readonly NuGetVersion _currentPanelVersion;

    public IReadOnlyList<IPluginReference> LoadedPlugins => _loadedPlugins;
    public IReadOnlyList<BootstrapStatusEntity> BootstrapState => _bootstrapState;
    public IReadOnlyList<PluginManifestEntity> PreInstalled => _preInstalled;
    public IReadOnlyList<string> RuntimeTargets => _runtimeTargets;
    public IReadOnlyList<PluginManifestEntity> ToApplyUpdates => _toApplyUpdates;
    public IReadOnlyList<PluginManifestEntity> Installed => _installed;
    public IReadOnlyList<PluginManifestEntity> Delete => _delete;
    public IReadOnlyList<PluginManifestEntity> Rollbacks => _rollbacks;

    internal PluginDiscovery(IPluginDiscoveryLocator locator, IPluginStateController pluginStateController)
    {
        _locator = locator;
        _pluginStateController = pluginStateController;
        var version = typeof(IPlugin).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion!;
        _currentPanelVersion = NuGetVersion.Parse(version);
    }



    public void Commit_All()
    {
        Commit_Delete();
        Commit_PreInstalled();
        Commit_Apply();
        Commit_Install();
        Commit_Runtime();
        CrazyReportConsole.ReportInfo("Loaded Plugin Cycle");
        CrazyReportConsole.ReportInfo($"Installed Found: {_installed.Count}");
        CrazyReportConsole.ReportInfo($"Mandatory PreInstalled Found: {_preInstalled.Count}");
    }
    public List<PluginManifestEntity> DiscoverPackages(string path)
    {
        string[] packages = Directory.GetFiles(path, "*.lpkg", SearchOption.TopDirectoryOnly);
        return packages.Select(PluginScannerExt.ReadManifestFromPackage).ToList();
    }

    public void Commit_Delete()
    {
        var dirDelete = _locator.PluginLifecycleDeleteLocation;
        var dirApply = _locator.PluginLifecycleApplyLocation;
        var dirInstalled = _locator.PluginLifecycleInstalledLocation;
        var dirRollback = _locator.PluginLifecycleRollbackLocation;
        var dirRuntime = _locator.PluginLifecycleRuntimeLocation;
        _delete = DiscoverPackages(dirDelete);

        string[] locationTargets = [dirDelete, dirApply, dirInstalled, dirRollback, dirRuntime];
        foreach (var targetDir in locationTargets)
        {
            foreach (var item in _delete)
            {
                string[] foundPackages = Directory.GetFiles(targetDir, $"*.lpkg", SearchOption.TopDirectoryOnly);
                foreach (var lpkg in foundPackages)
                {
                    var manifest = PluginScannerExt.ReadManifestFromPackage(lpkg);
                    if (manifest.Id.Equals(item.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        System.Console.WriteLine($"Deleting Package '{manifest.Id} v{manifest.Version}' ('{lpkg}')".Yellow());
                        File.Delete(lpkg);
                    }
                }
            }
        }
    }

    internal bool ValidateInstalledPackage(PluginManifestEntity testing, PluginManifestEntity current)
    {
        var testingVersion = NuGetVersion.Parse(testing.Version);
        var installedVersion = NuGetVersion.Parse(current.Version);
        var installedPanelVersion = NuGetVersion.Parse(current.PanelVersion);
        bool preInstalledMoreRecent = testingVersion > installedVersion;
        if (preInstalledMoreRecent)
            System.Console.WriteLine($"Incoming Package '{testing.Id} v{testing.Version}' is more recent than current '{current.Id} v{current.Version}'".Yellow());

        bool installedIncompatiblePanel = installedPanelVersion.Major != _currentPanelVersion.Major || installedPanelVersion > _currentPanelVersion;

        if (installedIncompatiblePanel)
            System.Console.WriteLine($"Current Package '{current.Id} v{installedVersion}' is more recent panel SDK is now incompatible.".Yellow());

        return !preInstalledMoreRecent && !installedIncompatiblePanel;
    }

    public bool PreInstalledRequiresRestore(string dir, PluginManifestEntity shipped, bool mustExist)
    {
        bool found = false;
        bool validated = true;
        string[] foundPackages = Directory.GetFiles(dir, $"*.lpkg", SearchOption.TopDirectoryOnly);
        foreach (var lpkg in foundPackages)
        {
            var manifest = PluginScannerExt.ReadManifestFromPackage(lpkg);
            var installedVersion = NuGetVersion.Parse(manifest.Version);
            var installedPanelVersion = NuGetVersion.Parse(manifest.PanelVersion);
            if (!manifest.Id.Equals(shipped.Id, StringComparison.OrdinalIgnoreCase))
                continue;
            bool delete = false;
            if (found)
            {
                System.Console.WriteLine($"'{manifest.Id} duplicated found, restoring...".Yellow());
                delete = true;
            }
            if (!delete)
                delete = !ValidateInstalledPackage(shipped, manifest);
            if (delete)
            {
                File.Delete(lpkg);
                validated = false;
            }
            if (!found)
                found = true;
        }
        return (mustExist && !found) || !validated;
    }
    public void ClearDirectoryFromPackage(string dir, PluginManifestEntity target)
    {
        string[] foundPackages = Directory.GetFiles(dir, $"*.lpkg", SearchOption.TopDirectoryOnly);
        foreach (var lpkg in foundPackages)
        {
            var manifest = PluginScannerExt.ReadManifestFromPackage(lpkg);
            if (!manifest.Id.Equals(target.Id, StringComparison.OrdinalIgnoreCase))
                continue;
            File.Delete(lpkg);
        }
    }

    public void SnapRollback(string dir, PluginManifestEntity target, bool purgeAfter)
    {
        var dirRollback = _locator.PluginLifecycleRollbackLocation;
        string[] foundPackages = Directory.GetFiles(dir, $"*.lpkg", SearchOption.TopDirectoryOnly);
        foreach (var lpkg in foundPackages)
        {
            var manifest = PluginScannerExt.ReadManifestFromPackage(lpkg);
            if (!manifest.Id.Equals(target.Id, StringComparison.OrdinalIgnoreCase))
                continue;

            var pkgMoveTarget = Path.Combine(dirRollback, ReconstructFileName(manifest));
            File.Move(lpkg, pkgMoveTarget, true);
            break;
        }

        if (purgeAfter)
            ClearDirectoryFromPackage(dir, target);
    }

    public void Commit_PreInstalled()
    {
        var dirApply = _locator.PluginLifecycleApplyLocation;
        var dirInstalled = _locator.PluginLifecycleInstalledLocation;
        var dirPreInstalled = _locator.PluginLifecyclePreInstalledLocation;
        _preInstalled = DiscoverPackages(dirPreInstalled);

        foreach (var preInstalled in _preInstalled)
        {
            bool restoreRequired = PreInstalledRequiresRestore(dirInstalled, preInstalled, true);
            if (restoreRequired)
            {
                ClearDirectoryFromPackage(dirApply, preInstalled);
                var pkgName = $"{preInstalled.Id}.{preInstalled.Version}.lpkg";
                var pkgMove = Path.Combine(dirPreInstalled, pkgName);
                var pkgMoveTarget = Path.Combine(dirApply, pkgName);
                File.Copy(pkgMove, pkgMoveTarget, true);
            }
        }
    }

    internal bool PreApplyValidation(PluginManifestEntity incoming)
    {
        var panelCompatibilities = NuGetVersion.Parse(incoming.PanelVersion);
        return _currentPanelVersion.Major == panelCompatibilities.Major && _currentPanelVersion >= panelCompatibilities;
    }
    internal bool PackageValidation(PluginManifestEntity incoming)
    {
        var panelCompatibilities = NuGetVersion.Parse(incoming.PanelVersion);
        return _currentPanelVersion.Major == panelCompatibilities.Major && _currentPanelVersion >= panelCompatibilities;
    }
    internal string ReconstructFileName(PluginManifestEntity manifest) => manifest.PackageFileName!;
    public void Commit_Apply()
    {
        var dirApply = _locator.PluginLifecycleApplyLocation;
        var dirInstalled = _locator.PluginLifecycleInstalledLocation;
        _toApplyUpdates = DiscoverPackages(dirApply);

        foreach (var toInstall in _toApplyUpdates)
        {

            bool valid = PreApplyValidation(toInstall);
            if (!valid)
            {
                System.Console.WriteLine($"{toInstall.Id} is not valid, removing.".Red());
                File.Delete(Path.Combine(dirApply, ReconstructFileName(toInstall)));
                continue;
            }
            SnapRollback(dirInstalled, toInstall, true);
            var pkgMove = Path.Combine(dirApply, ReconstructFileName(toInstall));
            var pkgMoveTarget = Path.Combine(dirInstalled, ReconstructFileName(toInstall));
            File.Move(pkgMove, pkgMoveTarget, true);
            ClearDirectoryFromPackage(dirApply, toInstall);
        }
    }

    public void Commit_Install()
    {
        var dirInstalled = _locator.PluginLifecycleInstalledLocation;
        var dirRuntime = _locator.PluginLifecycleRuntimeLocation;
        _installed = DiscoverPackages(dirInstalled);

        foreach (var toExtract in _installed)
        {
            var packageFile = toExtract.PackageFileLocation!;
            var runtimeLocation = Path.Combine(dirRuntime, toExtract.FolderId);
            var runtimeManifestFile = Path.Combine(dirRuntime, toExtract.FolderId, "manifest.json");
            bool valid = PackageValidation(toExtract);
            if (!valid)
            {
                System.Console.WriteLine($"{toExtract.Id} is not valid, removing.".Red());
                File.Delete(packageFile);
                if (Directory.Exists(runtimeLocation))
                    Directory.Delete(runtimeLocation, true);
                continue;
            }
            if (Directory.Exists(runtimeLocation) && !File.Exists(runtimeManifestFile))
                Directory.Delete(runtimeLocation, true);
            if (Directory.Exists(runtimeLocation))
            {
                var runtimeManifest = PluginScannerExt.ParseManifest(File.ReadAllText(runtimeManifestFile));
                var runtimeVersion = NuGetVersion.Parse(runtimeManifest.Version);
                var toExtractVersion = NuGetVersion.Parse(toExtract.Version);
                if (runtimeVersion == toExtractVersion) continue; // Cached extraction not needed.
            }

            System.Console.WriteLine($"Installing {toExtract.Id} v{toExtract.Version}.".Cyan());
            PluginScannerExt.ExtractPackage(packageFile, runtimeLocation);
        }
    }

    public void Commit_Runtime()
    {
        var dirRuntime = _locator.PluginLifecycleRuntimeLocation;
        string[] runtimeDirs = Directory.GetDirectories(dirRuntime);
        foreach (var toSeal in runtimeDirs)
        {
            var runtimeManifestFile = Path.Combine(toSeal, "manifest.json");
            var runtimeManifest = PluginScannerExt.ParseManifest(File.ReadAllText(runtimeManifestFile));
            var installed = _installed.SingleOrDefault(p => p.Id.Equals(runtimeManifest.Id, StringComparison.OrdinalIgnoreCase));
            if (installed != default)
            {
                _runtimeTargets.Add(toSeal);
                continue;
            }
            System.Console.WriteLine($"No longer installed {runtimeManifest.Id}, Removing ('{toSeal}').".Red());
            Directory.Delete(toSeal, true);
        }
    }


    public async Task Load_Runtime(IConfiguration configuration, CancellationToken ct = default)
    {
        _bootstrapState = await _pluginStateController.LoadFile();
        foreach (var toLoad in _runtimeTargets)
        {
            var runtimeManifestFile = Path.Combine(toLoad, "manifest.json");
            var manifestContent = File.ReadAllText(runtimeManifestFile);
            var runtimeManifest = PluginScannerExt.ParseManifest(manifestContent);
            var mandatory = _preInstalled.Any(p => p.Id.Equals(runtimeManifest.Id, StringComparison.OrdinalIgnoreCase));
            var committedState = _bootstrapState.SingleOrDefault(p => p.Id.Equals(runtimeManifest.Id, StringComparison.OrdinalIgnoreCase));
            if (committedState == default)
            {
                committedState = new BootstrapStatusEntity(runtimeManifest, mandatory ? BootstrapStartupState.Enabled : BootstrapStartupState.Disabled, null);
                _bootstrapState.Add(committedState);
            }

            if (committedState.StartupState == BootstrapStartupState.Disabled)
            {
                CrazyReportConsole.ReportWarning($"{runtimeManifest.Id} is disabled at startup.");
                continue;
            }
            // Force Enable PreInstalled
            if (committedState.StartupState == BootstrapStartupState.Disabled && mandatory)
            {
                CrazyReportConsole.ReportWarning($"{runtimeManifest.Id} is mandatory and cannot be disabled.");
                _bootstrapState.Remove(committedState);
                _bootstrapState.Add(committedState with { StartupState = mandatory ? BootstrapStartupState.Enabled : committedState.StartupState });
            }

            try
            {
                CrazyReportConsole.Report($"Creating Reference Loader for {runtimeManifest.Id}");
                var reference = PluginScannerExt.LoadPLugin(toLoad, [], DependencySettings.ScanSharedFrameworkNames());
                reference.Entry.Configure(configuration);
                _loadedPlugins.Add(reference);
                CrazyReportConsole.ReportInfo($"{runtimeManifest.Id} has loaded reference.");
            }
            catch (System.Exception ex)
            {
                CrazyReportConsole.ReportError($"Error Loading {runtimeManifest.Id} with {ex.Message}");
                _bootstrapState.Remove(committedState);
                committedState = committedState with { Failure = ex.Message };
                _bootstrapState.Add(committedState);
                continue;
            }
        }
        await _pluginStateController.SaveAsync(_bootstrapState);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                foreach (var item in _loadedPlugins)
                    item.Unload();

            }

            _bootstrapState = new();
            _preInstalled = new();
            _runtimeTargets = new();
            _toApplyUpdates = new();
            _installed = new();
            _delete = new();
            _rollbacks = new();
            _disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~PluginDiscovery()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
