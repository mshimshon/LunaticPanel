using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Engine.Plugin;
using LunaticPanel.Engine.Tests.Extensions;
using LunaticPanel.Engine.Web.Bootstraper;
using NuGet.Versioning;
using System;
using System.Reflection;

namespace LunaticPanel.Engine.Tests.Bootstraper;

[CollectionDefinition("NoParallel", DisableParallelization = true)]
public class NoParallelCollection { }

[Collection("NoParallel")]
public class PluginDiscoveryTests : IDisposable
{
    private readonly PluginDiscoveryLocation _pluginDiscoveryLocation;
    private readonly NuGetVersion _panelMinorDiff;
    private readonly NuGetVersion _panelMajorDiff;
    private readonly PluginDiscovery _pluginDiscovery;
    private readonly string _tmpRoot;
    private bool _disposedValue;

    public PluginDiscoveryTests()
    {

        _pluginDiscoveryLocation = new PluginDiscoveryLocation();
        if (_pluginDiscoveryLocation.IsLinux)
            _tmpRoot = $"{Path.DirectorySeparatorChar}";

        else
        {
            _tmpRoot = Path.Combine(Path.GetTempPath(), "lunaticpanel", "linux");
            if (Directory.Exists(_tmpRoot))
            {
                Directory.Delete(_tmpRoot, true);
            }
        }
        _pluginDiscoveryLocation.Initialize();
        _pluginDiscovery = new PluginDiscovery(_pluginDiscoveryLocation);


        var currentPanel = _pluginDiscovery._currentPanelVersion;
        var minorDiff = new Version(currentPanel.Major, currentPanel.Minor + 1, currentPanel.Patch, currentPanel.Revision);
        _panelMinorDiff = new(minorDiff, currentPanel.Release, currentPanel.Metadata);
        var majorDiff = new Version(currentPanel.Major + 1, currentPanel.Minor, currentPanel.Patch, currentPanel.Revision);
        _panelMajorDiff = new(majorDiff, currentPanel.Release, currentPanel.Metadata);
    }



    [Fact]
    public void Load_Runtime_ShouldFail()
    {
        string id = "Package.Core";
        string version = "1.0.0";
        var panelTarget = _pluginDiscovery._currentPanelVersion;
        var panelTargetString = panelTarget.ToString();
        var manifest = PackageExt.CreateFakePackage(id, id, version, panelTargetString, _pluginDiscoveryLocation.PluginLifecycleApplyLocation);
        var manifest2 = PackageExt.CreateFakePackage("Package1.Core1", id, version, panelTargetString, _pluginDiscoveryLocation.PluginLifecycleApplyLocation);
        _pluginDiscovery.Commit_All();
        _pluginDiscovery._bootstrapState.Add(new BootstrapStatusEntity(manifest, BootstrapStartupState.Disabled, null));
        _pluginDiscovery._bootstrapState.Add(new BootstrapStatusEntity(manifest2, BootstrapStartupState.Enabled, null));
        _pluginDiscovery.SaveBootstrapState();
        _pluginDiscovery.Load_Runtime();
        _pluginDiscovery.LoadBootstrapState();
        var currentState1 = _pluginDiscovery._bootstrapState.Single(p => p.Id == manifest.Id);
        var currentState2 = _pluginDiscovery._bootstrapState.Single(p => p.Id == manifest2.Id);
        Assert.True(currentState1.StartupState == BootstrapStartupState.Disabled);
        Assert.True(currentState2.StartupState == BootstrapStartupState.Enabled);
        Assert.True(currentState1.Failure == default);
        Assert.True(currentState2.Failure != default);

    }

    [Theory]
    [InlineData("Package.Core", "1.0.0", 0)]
    public void Commit_PreInstalled_ShouldAutoInstall(string id,
        string version,
        int panelVersion)
    {
        var panelTarget = panelVersion == 0 ? _pluginDiscovery._currentPanelVersion : panelVersion == 1 ? _panelMinorDiff : _panelMajorDiff;
        var panelTargetString = panelTarget.ToString();
        var manifest = PackageExt.CreateFakePackage(id, id, version, panelTargetString, _pluginDiscoveryLocation.PluginLifecyclePreInstalledLocation);
        _pluginDiscovery.Commit_PreInstalled();
        var file = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleApplyLocation, manifest.PackageFileName);
        Assert.True(File.Exists(file));
    }

    [Fact]
    public void Commit_PreInstalled_ShouldNotReInstall()
    {
        string id = "Package.Core";
        string version = "1.0.0";
        var panelTargetString = _pluginDiscovery._currentPanelVersion.ToString();
        var manifest = PackageExt.CreateFakePackage(id, id, version, panelTargetString, _pluginDiscoveryLocation.PluginLifecyclePreInstalledLocation);
        var manifestExisting = PackageExt.CreateFakePackage(id, id, "1.0.1", panelTargetString, _pluginDiscoveryLocation.PluginLifecycleInstalledLocation);
        _pluginDiscovery.Commit_PreInstalled();
        var expectedDestination = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleApplyLocation, manifest.PackageFileName);
        var expectedInstall = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleInstalledLocation, manifestExisting.PackageFileName);
        Assert.False(File.Exists(expectedDestination));
        Assert.True(File.Exists(expectedInstall));
    }

    [Fact]
    public void Commit_PreInstalled_ShouldPurgeReinstall_PanelIncompatible()
    {
        string id = "Package.Core";
        string version = "1.0.0";
        var panelTargetString = _pluginDiscovery._currentPanelVersion.ToString();
        var manifest = PackageExt.CreateFakePackage(id, id, version, panelTargetString, _pluginDiscoveryLocation.PluginLifecyclePreInstalledLocation);
        var manifestExisting = PackageExt.CreateFakePackage(id, id, "1.0.1", _panelMinorDiff.ToString(), _pluginDiscoveryLocation.PluginLifecycleInstalledLocation);
        _pluginDiscovery.Commit_PreInstalled();
        var expectedDestination = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleApplyLocation, manifest.PackageFileName);
        var expectedInstall = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleInstalledLocation, manifestExisting.PackageFileName);
        Assert.True(File.Exists(expectedDestination));
        Assert.False(File.Exists(expectedInstall));
    }

    [Fact]
    public void Commit_PreInstalled_ShouldPurgeReinstall_PanelIncompatibleMajor()
    {
        string id = "Package.Core";
        string version = "1.0.0";
        var panelTargetString = _pluginDiscovery._currentPanelVersion.ToString();
        var manifest = PackageExt.CreateFakePackage(id, id, version, panelTargetString, _pluginDiscoveryLocation.PluginLifecyclePreInstalledLocation);
        var manifestExisting = PackageExt.CreateFakePackage(id, id, "1.0.1", _panelMajorDiff.ToString(), _pluginDiscoveryLocation.PluginLifecycleInstalledLocation);
        _pluginDiscovery.Commit_PreInstalled();
        var expectedDestination = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleApplyLocation, manifest.PackageFileName);
        var expectedInstall = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleInstalledLocation, manifestExisting.PackageFileName);
        Assert.True(File.Exists(expectedDestination));
        Assert.False(File.Exists(expectedInstall));
    }

    [Fact]
    public void Commit_PreInstalled_ShouldPurgeReinstall_InstalledOlder()
    {
        string id = "Package.Core";
        string version = "1.0.0";
        var panelTargetString = _pluginDiscovery._currentPanelVersion.ToString();
        var manifest = PackageExt.CreateFakePackage(id, id, version, panelTargetString, _pluginDiscoveryLocation.PluginLifecyclePreInstalledLocation);
        var manifestExisting = PackageExt.CreateFakePackage(id, id, "1.0.0-beta.1", panelTargetString, _pluginDiscoveryLocation.PluginLifecycleInstalledLocation);
        _pluginDiscovery.Commit_PreInstalled();
        var expectedDestination = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleApplyLocation, manifest.PackageFileName);
        var expectedInstall = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleInstalledLocation, manifestExisting.PackageFileName);
        Assert.True(File.Exists(expectedDestination));
        Assert.False(File.Exists(expectedInstall));
    }

    [Theory]
    [InlineData("Package.Core", "1.0.0", 0)]
    public void Commit_Apply_ShouldInstall(string id,
        string version,
        int panelVersion)
    {
        var panelTarget = panelVersion == 0 ? _pluginDiscovery._currentPanelVersion : panelVersion == 1 ? _panelMinorDiff : _panelMajorDiff;
        var panelTargetString = panelTarget.ToString();
        var manifest = PackageExt.CreateFakePackage(id, id, version, panelTargetString, _pluginDiscoveryLocation.PluginLifecycleApplyLocation);
        _pluginDiscovery.Commit_Apply();
        var file = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleInstalledLocation, manifest.PackageFileName);
        var oldLocation = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleApplyLocation, manifest.PackageFileName);
        Assert.True(File.Exists(file));
        Assert.False(File.Exists(oldLocation));
    }

    [Fact]
    public void Commit_Apply_ShouldInstallAndRollback()
    {
        string id = "Package.Core";
        string version = "1.0.0";
        var panelTargetString = _pluginDiscovery._currentPanelVersion.ToString();
        var manifest = PackageExt.CreateFakePackage(id, id, version, panelTargetString, _pluginDiscoveryLocation.PluginLifecycleApplyLocation);
        var manifest2 = PackageExt.CreateFakePackage(id, id, "2.0.0", panelTargetString, _pluginDiscoveryLocation.PluginLifecycleInstalledLocation);
        _pluginDiscovery.Commit_Apply();
        var file = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleInstalledLocation, manifest.PackageFileName);
        var fileRollback = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleRollbackLocation, manifest2.PackageFileName);
        Assert.True(File.Exists(file));
        Assert.False(File.Exists(manifest2.PackageFileLocation));
        Assert.True(File.Exists(fileRollback));
    }



    [Fact]
    public void Commit_Apply_ShouldNotInstall_Invalid()
    {
        string id = "Package.Core";
        string version = "1.0.0";
        var panelTargetString = _pluginDiscovery._currentPanelVersion.ToString();
        var manifest = PackageExt.CreateFakePackage(id, id, version, _panelMajorDiff.ToString(), _pluginDiscoveryLocation.PluginLifecycleApplyLocation);
        _pluginDiscovery.Commit_Apply();
        var file = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleInstalledLocation, manifest.PackageFileName);
        var file2 = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleApplyLocation, manifest.PackageFileName);
        Assert.False(File.Exists(file));
        Assert.False(File.Exists(file2));
    }


    [Fact]
    public void Commit_Installed_ShouldDoNothing()
    {
        string id = "Package.Core";
        string version = "1.0.0";
        var panelTargetString = _pluginDiscovery._currentPanelVersion.ToString();
        var manifest = PackageExt.CreateFakePackage(id, id, version, panelTargetString, _pluginDiscoveryLocation.PluginLifecycleInstalledLocation);
        _pluginDiscovery.Commit_Install();
        var expectedDestination = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleRuntimeLocation, manifest.FolderId);
        Assert.True(Directory.Exists(expectedDestination));
        Assert.True(File.Exists(manifest.PackageFileLocation));
    }

    [Fact]
    public void Commit_Installed_ShouldReExtractDifferentVersion()
    {
        string id = "Package.Core";
        string version = "1.0.0";
        var panelTargetString = _pluginDiscovery._currentPanelVersion.ToString();
        var manifest = PackageExt.CreateFakePackage(id, id, version, panelTargetString, _pluginDiscoveryLocation.PluginLifecycleInstalledLocation);
        var manifest2 = PackageExt.CreateFakePackage(id, id, "1.0.1", panelTargetString, _pluginDiscoveryLocation.PluginLifecycleLocation);
        var expectedDestination = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleRuntimeLocation, manifest.FolderId);
        PluginScannerExt.ExtractPackage(manifest2.PackageFileLocation!, expectedDestination);
        _pluginDiscovery.Commit_Install();
        var manifestFile = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleRuntimeLocation, manifest.FolderId, "manifest.json");
        var current = PluginScannerExt.ParseManifest(File.ReadAllText(manifestFile));
        Assert.True(Directory.Exists(expectedDestination));
        Assert.True(File.Exists(manifest.PackageFileLocation));
        Assert.True(current.Version == manifest.Version);
        Assert.True(current.Version != manifest2.Version);
    }

    [Fact]
    public void Commit_Installed_ShouldInstall_Missing()
    {
        string id = "Package.Core";
        string version = "1.0.0";
        var panelTargetString = _pluginDiscovery._currentPanelVersion.ToString();
        var manifest = PackageExt.CreateFakePackage(id, id, version, panelTargetString, _pluginDiscoveryLocation.PluginLifecycleInstalledLocation);
        var expectedDestination = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleRuntimeLocation, manifest.FolderId);
        PluginScannerExt.ExtractPackage(manifest.PackageFileLocation!, expectedDestination);
        _pluginDiscovery.Commit_Install();
        Assert.True(Directory.Exists(expectedDestination));
        Assert.True(File.Exists(manifest.PackageFileLocation));
    }



    [Fact]
    public void Commit_Installed_ShouldClear_ValidationFailed()
    {
        string id = "Package.Core";
        string version = "1.0.0";
        var manifest = PackageExt.CreateFakePackage(id, id, version, _panelMajorDiff.ToString(), _pluginDiscoveryLocation.PluginLifecycleInstalledLocation);
        var expectedDestination = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleRuntimeLocation, manifest.FolderId);
        PluginScannerExt.ExtractPackage(manifest.PackageFileLocation!, expectedDestination);
        _pluginDiscovery.Commit_Install();
        Assert.False(Directory.Exists(expectedDestination));
        Assert.False(File.Exists(manifest.PackageFileLocation));
    }


    [Fact]
    public void Commit_Installed_ShouldReExtract_Corrupted()
    {
        string id = "Package.Core";
        string version = "1.0.0";
        var manifest = PackageExt.CreateFakePackage(id, id, version, _pluginDiscovery._currentPanelVersion.ToString(), _pluginDiscoveryLocation.PluginLifecycleInstalledLocation);
        var expectedDestination = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleRuntimeLocation, manifest.FolderId);
        PluginScannerExt.ExtractPackage(manifest.PackageFileLocation!, expectedDestination);
        var manifestFile = Path.Combine(expectedDestination, "manifest.json");
        File.Delete(manifestFile);
        _pluginDiscovery.Commit_Install();
        Assert.True(Directory.Exists(expectedDestination));
        Assert.True(File.Exists(manifestFile));
        Assert.True(File.Exists(manifest.PackageFileLocation));
    }

    [Fact]
    public void TargetFolderLocations_ShouldPass()
    {

        var expectedBaseLocation = Path.Combine(_tmpRoot, "var", "lib", "lunaticpanel", ".plugins");
        Assert.Equal(expectedBaseLocation, _pluginDiscoveryLocation.PluginLifecycleLocation);
        Assert.Equal(Path.Combine(expectedBaseLocation, "apply"), _pluginDiscoveryLocation.PluginLifecycleApplyLocation);
        Assert.Equal(Path.Combine(expectedBaseLocation, "delete"), _pluginDiscoveryLocation.PluginLifecycleDeleteLocation);
        Assert.Equal(Path.Combine(expectedBaseLocation, "rollbacks"), _pluginDiscoveryLocation.PluginLifecycleRollbackLocation);
        Assert.Equal(Path.Combine(expectedBaseLocation, "installed"), _pluginDiscoveryLocation.PluginLifecycleInstalledLocation);
        Assert.Equal(Path.Combine(expectedBaseLocation, "runtime"), _pluginDiscoveryLocation.PluginLifecycleRuntimeLocation);
        var expectedPreinstallLocation = Path.Combine(_tmpRoot, "srv", "lunaticpanel", "plugins");
        Assert.Equal(expectedPreinstallLocation, _pluginDiscoveryLocation.PluginLifecyclePreInstalledLocation);
    }



    [Fact]
    public void Commit_Runtime_ShouldClearUninstalled()
    {
        string id = "Package.Core";
        string version = "1.0.0";
        var panelTargetString = _pluginDiscovery._currentPanelVersion.ToString();
        var manifest = PackageExt.CreateFakePackage(id, id, version, panelTargetString, _pluginDiscoveryLocation.PluginLifecycleInstalledLocation);
        var expectedDestination1 = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleRuntimeLocation, manifest.FolderId);
        var manifest2 = PackageExt.CreateFakePackage("My.Plugin2", id, version, panelTargetString, _pluginDiscoveryLocation.PluginLifecycleInstalledLocation);
        var expectedDestination2 = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleRuntimeLocation, manifest2.FolderId);
        var manifest3 = PackageExt.CreateFakePackage("My.Plugin3", id, version, panelTargetString, _pluginDiscoveryLocation.PluginLifecycleInstalledLocation);
        var expectedDestination3 = Path.Combine(_pluginDiscoveryLocation.PluginLifecycleRuntimeLocation, manifest3.FolderId);
        PluginScannerExt.ExtractPackage(manifest.PackageFileLocation!, expectedDestination1);
        PluginScannerExt.ExtractPackage(manifest2.PackageFileLocation!, expectedDestination2);
        PluginScannerExt.ExtractPackage(manifest3.PackageFileLocation!, expectedDestination3);
        File.Delete(manifest3.PackageFileLocation!);
        _pluginDiscovery.Commit_Install();
        _pluginDiscovery.Commit_Runtime();
        Assert.True(Directory.Exists(expectedDestination1));
        Assert.True(Directory.Exists(expectedDestination2));
        Assert.False(Directory.Exists(expectedDestination3));
        Assert.True(File.Exists(manifest.PackageFileLocation));
        Assert.True(File.Exists(manifest2.PackageFileLocation));
        Assert.False(File.Exists(manifest3.PackageFileLocation));
        var runtimeEntryPoints = new List<string>()
        {
            expectedDestination1,
            expectedDestination2,
        };

        Assert.Equivalent(runtimeEntryPoints, _pluginDiscovery._runtimeTargets);
    }




    [Fact]
    public void ClearDirectoryFromPackage_ShouldRemoveTargetPackageAllVersions()
    {
        string id = "Package.Core";
        string version = "1.0.0";
        var panelTargetString = _pluginDiscovery._currentPanelVersion.ToString();
        var manifest = PackageExt.CreateFakePackage(id, id, version, panelTargetString, _pluginDiscoveryLocation.PluginLifecycleInstalledLocation);
        var manifest2 = PackageExt.CreateFakePackage("My.Plugin2", id, version, panelTargetString, _pluginDiscoveryLocation.PluginLifecycleInstalledLocation);
        var manifest3 = PackageExt.CreateFakePackage("My.Plugin3", id, version, panelTargetString, _pluginDiscoveryLocation.PluginLifecycleInstalledLocation);
        _pluginDiscovery.ClearDirectoryFromPackage(_pluginDiscoveryLocation.PluginLifecycleInstalledLocation, manifest);
        Assert.False(File.Exists(manifest.PackageFileLocation));
        Assert.True(File.Exists(manifest2.PackageFileLocation));
        Assert.True(File.Exists(manifest3.PackageFileLocation));
    }


    [Fact]
    public void PreInstalledRequiresRestore_ShouldRemoveDuplicateMarkedInvalid()
    {
        string id = "Package.Core";
        string version = "1.0.0";
        var panelTargetString = _pluginDiscovery._currentPanelVersion.ToString();
        var manifest = PackageExt.CreateFakePackage(id, id, version, panelTargetString, _pluginDiscoveryLocation.PluginLifecycleInstalledLocation);
        var manifest2 = PackageExt.CreateFakePackage(id, id, "1.2.0", panelTargetString, _pluginDiscoveryLocation.PluginLifecycleInstalledLocation);
        bool requiresRestore = _pluginDiscovery.PreInstalledRequiresRestore(_pluginDiscoveryLocation.PluginLifecycleInstalledLocation, manifest, true);
        Assert.True(File.Exists(manifest.PackageFileLocation));
        Assert.False(File.Exists(manifest2.PackageFileLocation));
        Assert.True(requiresRestore);
    }



    [Theory]
    [InlineData("test", "1.0.0", "1.0.0", null)]
    [InlineData("test", "1.0.0", "1.0.1", null)]
    [InlineData("test", "1.0.0", "1.1.0", null)]
    [InlineData("test", "1.0.0", "2.0.0", null)]
    [InlineData("test", "1.1.0", "2.0.0", -1)]
    public void ValidateInstalledPackage_ShouldPass(string id,
        string incomingVersion,
        string currentVersion,
        int? minorVariation)
    {
        var currentPanel = _pluginDiscovery._currentPanelVersion;
        if (minorVariation != default)
        {
            var v = new Version(currentPanel.Major, (int)(currentPanel.Minor + minorVariation)!, currentPanel.Patch, currentPanel.Revision);
            currentPanel = new(v, currentPanel.Release, currentPanel.Metadata);
        }
        var incoming = PackageExt.CreateManifest(id, id, incomingVersion, _pluginDiscovery._currentPanelVersion.ToString());
        var current = PackageExt.CreateManifest(id, id, currentVersion, currentPanel.ToString());
        bool valid = _pluginDiscovery.ValidateInstalledPackage(incoming, current);
        Assert.True(valid);
    }

    [Theory]
    [InlineData("test", "1.0.0", "1.0.0", 1)]
    [InlineData("test", "1.0.0", "1.0.1", 2)]
    [InlineData("test", "1.2.0", "1.1.0", null)]
    [InlineData("test", "3.0.0", "2.0.0", null)]
    [InlineData("test", "3.1.0", "2.0.0", null)]
    public void ValidateInstalledPackage_ShouldFail(string id,
        string incomingVersion,
        string currentVersion,
        int? minorVariation)
    {
        var currentPanel = _pluginDiscovery._currentPanelVersion;
        if (minorVariation != default)
        {
            var v = new Version(currentPanel.Major, (int)(currentPanel.Minor + minorVariation)!, currentPanel.Patch, currentPanel.Revision);
            currentPanel = new(v, currentPanel.Release, currentPanel.Metadata);
        }
        var incoming = PackageExt.CreateManifest(id, id, incomingVersion, _pluginDiscovery._currentPanelVersion.ToString());
        var current = PackageExt.CreateManifest(id, id, currentVersion, currentPanel.ToString());
        bool valid = _pluginDiscovery.ValidateInstalledPackage(incoming, current);
        Assert.False(valid);
    }






    [Theory]
    [InlineData(null, -1)]
    [InlineData(null, -2)]
    [InlineData(null, null)]
    public void PreApplyValidation_ShouldPass(int? majorVariation, int? minorVariation)
    {
        var currentPanel = _pluginDiscovery._currentPanelVersion;
        if (minorVariation != default)
        {
            var v = new Version(currentPanel.Major, (int)(currentPanel.Minor + minorVariation)!, currentPanel.Patch, currentPanel.Revision);
            currentPanel = new(v, currentPanel.Release, currentPanel.Metadata);
        }

        if (majorVariation != default)
        {
            var v = new Version((int)(currentPanel.Major + majorVariation)!, currentPanel.Minor, currentPanel.Patch, currentPanel.Revision);
            currentPanel = new(v, currentPanel.Release, currentPanel.Metadata);
        }
        var incoming = PackageExt.CreateManifest("Test", "Test", "1.0.0", currentPanel.ToString());
        bool valid = _pluginDiscovery.PreApplyValidation(incoming);
        Assert.True(valid);
    }

    [Theory]
    [InlineData(null, 1)]
    [InlineData(null, 2)]
    [InlineData(1, null)]
    public void PreApplyValidation_ShouldFail(int? majorVariation, int? minorVariation)
    {
        var currentPanel = _pluginDiscovery._currentPanelVersion;
        if (minorVariation != default)
        {
            var v = new Version(currentPanel.Major, (int)(currentPanel.Minor + minorVariation)!, currentPanel.Patch, currentPanel.Revision);
            currentPanel = new(v, currentPanel.Release, currentPanel.Metadata);
        }

        if (majorVariation != default)
        {
            var v = new Version((int)(currentPanel.Major + majorVariation)!, currentPanel.Minor, currentPanel.Patch, currentPanel.Revision);
            currentPanel = new(v, currentPanel.Release, currentPanel.Metadata);
        }
        var incoming = PackageExt.CreateManifest("Test", "Test", "1.0.0", currentPanel.ToString());
        bool valid = _pluginDiscovery.PreApplyValidation(incoming);
        Assert.False(valid);
    }

    // [Fact]
    // public void DiscoverPackages_ShouldPass()
    // {
    //     _pluginDiscovery._toApplyUpdates = _pluginDiscovery.DiscoverPackages(_pluginDiscoveryLocation.PluginLifecycleApplyLocation);
    //     Assert.True(_pluginDiscovery._toApplyUpdates.Count == 0);
    // }

    [Fact]
    public void PanelVersion_ShouldPass()
    {
        var version = typeof(IPlugin).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        var panelV = NuGetVersion.Parse(version!);
        Assert.True(_pluginDiscovery._currentPanelVersion == panelV);
    }



    private void CleanUpJob()
    {
        Directory.Delete(_pluginDiscoveryLocation.PluginLifecycleLocation, true);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                CleanUpJob();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~PluginDiscoveryTests()
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
