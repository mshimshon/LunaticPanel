using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Plugin;
using LunaticPanel.Engine.Tests.Extensions;
using LunaticPanel.Engine.Bootstraper.Engine;
using NuGet.Versioning;
using System;
using System.Reflection;
using LunaticPanel.Engine.Bootstraper.Discovery;

namespace LunaticPanel.Engine.Tests.Bootstraper;



[Collection("NoParallel")]
public class UpdaterDiscoveryTests : IDisposable
{
    private readonly UpdateDiscovery _updateDiscovery;
    private readonly PluginDiscoveryLocation _pluginDiscoveryLocation;
    private readonly UpdateDiscoveryLocation _updateDiscoveryLocation;
    private bool _disposedValue;
    private string _fakePanelRoot = Pathf.CombineReady(Pathf.DirectorySeparatorChar, "tmp", "lunaticpanel", "xunit", "fakePanel");
    public UpdaterDiscoveryTests()
    {

        _updateDiscoveryLocation = new UpdateDiscoveryLocation();

        _updateDiscoveryLocation.Initialize();
        _pluginDiscoveryLocation = new PluginDiscoveryLocation();
        _pluginDiscoveryLocation.Initialize();
        _updateDiscovery = new UpdateDiscovery(_updateDiscoveryLocation, _pluginDiscoveryLocation);

    }


    private async Task CreateUpgrade()
    {
        // Create Fake Panel Bin to Pack
        var file1 = Path.GetTempFileName();
        var filename1 = Path.GetFileName(file1);
        var file2 = Path.GetTempFileName();
        var filename2 = Path.GetFileName(file2);

        File.Move(file1, Pathf.Combine(_fakePanelRoot, filename1));

        // Simulate Content for Existing Panel
        PackageExt.CreateFakePackage("My.Plugin2", "whatever", "1.0.0", "10.0.0", _pluginDiscoveryLocation.PluginLifecyclePreInstalledLocation);
        File.Move(file2, Pathf.Combine(_updateDiscoveryLocation.BinaryPathRoot, filename2));

        var panelFile = Pathf.Combine(_updateDiscoveryLocation.UpgradeReadyApply, "lunaticpanel_latest.tar.gz");
        await ArchiveUtilityExt.PackTarGz(_fakePanelRoot, panelFile);
        // Create Version File Mark
        var version = Pathf.Combine(_updateDiscoveryLocation.UpgradeReadyApply, "version.txt");
        File.WriteAllText(version, "1.0.0");
        var mark1 = Path.GetTempFileName();
        File.Copy(mark1, _updateDiscovery.AccessBinMark);
        File.Copy(mark1, _updateDiscovery.AccessPluginRootMark);
        File.Move(mark1, _updateDiscovery.AccessUpgradeRootAccessMark);

        PackageExt.CreateFakePackage("My.Plugin2", "whatever", "1.0.0", "10.0.0", _updateDiscoveryLocation.UpgradePluginPreinstalled);
        PackageExt.CreateFakePackage("My.Plugin", "whatever", "1.0.0", "10.0.0", _updateDiscoveryLocation.UpgradePluginInstalled);
    }

    [Fact]
    public async Task Panel_ShouldBe_Upgradable()
    {
        await CreateUpgrade();
        bool canUpgrade = _updateDiscovery.CanUpgrade();
        Assert.True(canUpgrade);
    }

    [Fact]
    public async Task Panel_ShouldNotBe_Upgradable_ApplyPending()
    {
        await CreateUpgrade();
        PackageExt.CreateFakePackage("My.Plugin2", "whatever", "1.0.0", "10.0.0", _pluginDiscoveryLocation.PluginLifecycleApplyLocation);

        bool canUpgrade = _updateDiscovery.CanUpgrade();
        Assert.False(canUpgrade);
    }

    [Fact]
    public async Task Panel_ShouldNotBe_Upgradable_DeletePending()
    {
        await CreateUpgrade();
        PackageExt.CreateFakePackage("My.Plugin2", "whatever", "1.0.0", "10.0.0", _pluginDiscoveryLocation.PluginLifecycleDeleteLocation);

        bool canUpgrade = _updateDiscovery.CanUpgrade();
        Assert.False(canUpgrade);
    }


    [Fact]
    public async Task Panel_Should_Upgrade()
    {
        // Create Fake Panel Bin to Pack
        var file1 = Path.GetTempFileName();
        var filename1 = Path.GetFileName(file1);
        var file2 = Path.GetTempFileName();
        var filename2 = Path.GetFileName(file2);
        File.Move(file1, Pathf.Combine(_fakePanelRoot, filename1));

        // Simulate Content for Existing Panel
        PackageExt.CreateFakePackage("My.Plugin2", "whatever", "1.0.0-beta.1", "10.0.0", _pluginDiscoveryLocation.PluginLifecyclePreInstalledLocation);
        File.Move(file2, Pathf.Combine(_updateDiscoveryLocation.BinaryPathRoot, filename2));

        var panelFile = Pathf.Combine(_updateDiscoveryLocation.UpgradeReadyApply, "lunaticpanel_latest.tar.gz");
        await ArchiveUtilityExt.PackTarGz(_fakePanelRoot, panelFile, TestContext.Current.CancellationToken);
        // Create Version File Mark
        var version = Pathf.Combine(_updateDiscoveryLocation.UpgradeReadyApply, "version.txt");
        File.WriteAllText(version, "1.0.0");
        var mark1 = Path.GetTempFileName();
        File.Copy(mark1, _updateDiscovery.AccessBinMark);
        File.Copy(mark1, _updateDiscovery.AccessPluginRootMark);
        File.Move(mark1, _updateDiscovery.AccessUpgradeRootAccessMark);

        var preinstalled = PackageExt.CreateFakePackage("My.Plugin2", "whatever", "1.0.0", "10.0.0", _updateDiscoveryLocation.UpgradePluginPreinstalled);
        var userInstalled = PackageExt.CreateFakePackage("My.Plugin", "whatever", "1.0.0", "10.0.0", _updateDiscoveryLocation.UpgradePluginInstalled);
        await _updateDiscovery.StartUpgrade();

        var expectedInstalledFile = Pathf.Combine(_updateDiscoveryLocation.BinaryPathRoot, filename1);
        var expectedPreInstalledPlugin = Pathf.Combine(_pluginDiscoveryLocation.PluginLifecyclePreInstalledLocation, preinstalled.PackageFileName);
        var expectedApplyPlugin1 = Pathf.Combine(_pluginDiscoveryLocation.PluginLifecycleApplyLocation, preinstalled.PackageFileName);
        var expectedApplyPlugin2 = Pathf.Combine(_pluginDiscoveryLocation.PluginLifecycleApplyLocation, userInstalled.PackageFileName);
        Assert.True(File.Exists(expectedInstalledFile));
        Assert.True(File.Exists(expectedPreInstalledPlugin));
        Assert.True(File.Exists(expectedApplyPlugin1));
        Assert.True(File.Exists(expectedApplyPlugin2));
    }


    [Fact]
    public async Task Panel_Should_RestoreBackup()
    {
        var file2 = Path.GetTempFileName();
        var filename2 = Path.GetFileName(file2);

        // Simulate Content for Existing Panel
        var plugin0 = PackageExt.CreateFakePackage("My.Plugin2", "whatever", "1.0.0-beta.1", "10.0.0", _pluginDiscoveryLocation.PluginLifecyclePreInstalledLocation);
        File.Move(file2, Pathf.Combine(_updateDiscoveryLocation.BinaryPathRoot, filename2));
        var plugin1 = PackageExt.CreateFakePackage("My.Plugin2", "whatever", "1.0.0", "10.0.0", _pluginDiscoveryLocation.PluginLifecycleInstalledLocation);


        await _updateDiscovery.BackUpCurrentPanel();

        var expectedInstalledPlugin = Pathf.Combine(_pluginDiscoveryLocation.PluginLifecycleInstalledLocation, plugin1.PackageFileName);
        var expectedPreInstalledPlugin = Pathf.Combine(_pluginDiscoveryLocation.PluginLifecyclePreInstalledLocation, plugin0.PackageFileName);
        var expectedBinFile = Pathf.Combine(_updateDiscoveryLocation.BinaryPathRoot, filename2);
        Assert.True(File.Exists(expectedInstalledPlugin));
        Assert.True(File.Exists(expectedPreInstalledPlugin));
        Assert.True(File.Exists(expectedBinFile));
        Assert.True(File.Exists(_updateDiscovery._failSafePanelFile));
        Assert.True(File.Exists(_updateDiscovery._failSafePluginFile));

        File.Delete(expectedInstalledPlugin);
        File.Delete(expectedPreInstalledPlugin);
        File.Delete(expectedBinFile);
        await _updateDiscovery.FailSafeRestoring();
        Assert.True(File.Exists(expectedInstalledPlugin));
        Assert.True(File.Exists(expectedPreInstalledPlugin));
        Assert.True(File.Exists(expectedBinFile));
        Assert.False(File.Exists(_updateDiscovery._failSafePanelFile));
        Assert.False(File.Exists(_updateDiscovery._failSafePluginFile));
    }

    [Fact]
    public void UpgradeFileLocations_ShouldPass()
    {

        var expectedBinary = Pathf.Combine(Pathf.DirectorySeparatorChar, "srv", "lunaticpanel");
        var expectedVarLibRoot = Pathf.Combine(Pathf.DirectorySeparatorChar, "var", "lib", "lunaticpanel");
        var expectedPluginRoot = Pathf.Combine(expectedVarLibRoot, ".plugins");
        var expectedUpgradeRoot = Pathf.Combine(expectedVarLibRoot, ".upgrade");
        var expectedUpgradeReadyApply = Pathf.Combine(expectedUpgradeRoot, "apply");

        var expectedUpgradeMarkerFile = Pathf.Combine(expectedUpgradeReadyApply, "version.txt");
        Assert.Equal(expectedUpgradeMarkerFile, _updateDiscovery.UpgradeMarkerFile);

        var expectedAccessBinMark = Pathf.Combine(expectedBinary, "access.test");
        Assert.Equal(expectedAccessBinMark, _updateDiscovery.AccessBinMark);

        var expectedAccessUpgradeRootAccessMark = Pathf.Combine(expectedUpgradeRoot, "access.test");
        Assert.Equal(expectedAccessUpgradeRootAccessMark, _updateDiscovery.AccessUpgradeRootAccessMark);

        var expectedAccessPluginRootMark = Pathf.Combine(expectedPluginRoot, "access.test");
        Assert.Equal(expectedAccessPluginRootMark, _updateDiscovery.AccessPluginRootMark);

    }

    [Fact]
    public void TargetFolderLocations_ShouldPass()
    {
        var expectedBinary = Pathf.Combine(Pathf.DirectorySeparatorChar, "srv", "lunaticpanel");
        Assert.Equal(expectedBinary, _updateDiscoveryLocation.BinaryPathRoot);

        var expectedVarLibRoot = Pathf.Combine(Pathf.DirectorySeparatorChar, "var", "lib", "lunaticpanel");
        Assert.Equal(expectedVarLibRoot, _updateDiscoveryLocation.VarLibRoot);

        var expectedUpgradeRoot = Pathf.Combine(expectedVarLibRoot, ".upgrade");
        Assert.Equal(expectedUpgradeRoot, _updateDiscoveryLocation.UpgradeRoot);

        var expectedUpgradePluginCache = Pathf.Combine(expectedUpgradeRoot, "plugins");
        Assert.Equal(expectedUpgradePluginCache, _updateDiscoveryLocation.UpgradePluginCache);

        var expectedUpgradePluginInstalled = Pathf.Combine(expectedUpgradeRoot, "apply", "installed");
        Assert.Equal(expectedUpgradePluginInstalled, _updateDiscoveryLocation.UpgradePluginInstalled);

        var expectedUpgradePluginPreinstalled = Pathf.Combine(expectedUpgradeRoot, "apply", "preinstalled");
        Assert.Equal(expectedUpgradePluginPreinstalled, _updateDiscoveryLocation.UpgradePluginPreinstalled);

        var expectedUpgradeReadyApply = Pathf.Combine(expectedUpgradeRoot, "apply");
        Assert.Equal(expectedUpgradeReadyApply, _updateDiscoveryLocation.UpgradeReadyApply);

        var expectedRuntimeBackupRoot = Pathf.Combine(expectedVarLibRoot, "upgrade_backup");
        Assert.Equal(expectedRuntimeBackupRoot, _updateDiscoveryLocation.RuntimeBackupRoot);
    }



    private void CleanUpJob()
    {
        if (Directory.Exists(_updateDiscoveryLocation.UpgradeRoot))
            Directory.Delete(_updateDiscoveryLocation.UpgradeRoot, true);

        if (Directory.Exists(_updateDiscoveryLocation.BinaryPathRoot))
            Directory.Delete(_updateDiscoveryLocation.BinaryPathRoot, true);


        if (Directory.Exists(_pluginDiscoveryLocation.PluginLifecycleLocation))
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
