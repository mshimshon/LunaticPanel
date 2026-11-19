using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Tests.Extensions;
using LunaticPanel.Engine.Bootstraper;
using LunaticPanel.Engine.Bootstraper.Discovery.Services;
using LunaticPanel.Engine.Services;
using System;

namespace LunaticPanel.Engine.Tests.Bootstraper.Services;

[Collection("NoParallel")]
public class PluginStateControllerTests : IDisposable
{
    private readonly PluginStateController _pluginStateController;
    private readonly string _bootstrapFileLocation;
    private bool _disposedValue;
    private List<BootstrapStatusEntity> _bootstrapEntities = new()
    {
      new BootstrapStatusEntity(PackageExt.CreateManifest("Package.Core", "Package Core", "10.0.0", "1.0.0"), BootstrapStartupState.Disabled, null),
      new BootstrapStatusEntity(PackageExt.CreateManifest("Package.Main", "Package Master", "10.0.0", "1.0.0"), BootstrapStartupState.Disabled, null),
      new BootstrapStatusEntity(PackageExt.CreateManifest("Package.Web", "Package Web", "10.0.0", "1.0.0"), BootstrapStartupState.Enabled, "Failure"),
      new BootstrapStatusEntity(PackageExt.CreateManifest("Package.Log", "Package Logging", "10.0.0", "1.0.0"), BootstrapStartupState.Enabled, null),
    };
    public PluginStateControllerTests()
    {
        _pluginStateController = new PluginStateController();
        _bootstrapFileLocation = Pathf.Combine(Pathf.DirectorySeparatorChar, "var", "lib", "lunaticpanel", IBootstrapFileController.BootstrapFilename);
    }

    [Fact]
    public void FileAutoCreated()
    {
        Assert.True(File.Exists(_bootstrapFileLocation));
    }

    [Fact]
    public async Task LoadEmpty_Success()
    {
        var list = await _pluginStateController.LoadFile(TestContext.Current.CancellationToken);
        Assert.True(list.Count == 0);
    }


    [Fact]
    public async Task WriteBootstrap_Success()
    {
        await _pluginStateController.SaveAsync(_bootstrapEntities, TestContext.Current.CancellationToken);
        var list = await _pluginStateController.LoadFile(TestContext.Current.CancellationToken);

        Assert.Equivalent(_bootstrapEntities, list);
    }


    [Fact]
    public async Task EnablePlugin_Success()
    {
        await _pluginStateController.SaveAsync(_bootstrapEntities, TestContext.Current.CancellationToken);
        var list = await _pluginStateController.LoadFile(TestContext.Current.CancellationToken);
        var target = _bootstrapEntities.First();
        _bootstrapEntities.Remove(target);
        _bootstrapEntities.Add(target with { StartupState = BootstrapStartupState.Enabled });
        await _pluginStateController.EnablePlugin(target.Id, TestContext.Current.CancellationToken);
        list = await _pluginStateController.LoadFile(TestContext.Current.CancellationToken);
        Assert.Equivalent(_bootstrapEntities, list);
    }

    [Fact]
    public async Task DisablePlugin_Success()
    {
        await _pluginStateController.SaveAsync(_bootstrapEntities, TestContext.Current.CancellationToken);
        var list = await _pluginStateController.LoadFile(TestContext.Current.CancellationToken);
        var target = _bootstrapEntities.First();
        _bootstrapEntities.Remove(target);
        _bootstrapEntities.Add(target with { StartupState = BootstrapStartupState.Disabled });
        await _pluginStateController.DisablePlugin(target.Id, TestContext.Current.CancellationToken);
        list = await _pluginStateController.LoadFile(TestContext.Current.CancellationToken);
        Assert.Equivalent(_bootstrapEntities, list);
    }



    [Fact]
    public async Task EnablePlugin_NoActionMissingPlugin_Success()
    {
        await _pluginStateController.SaveAsync(_bootstrapEntities, TestContext.Current.CancellationToken);
        var list = await _pluginStateController.LoadFile(TestContext.Current.CancellationToken);
        await _pluginStateController.EnablePlugin("whatever.dfdd", TestContext.Current.CancellationToken);
        Assert.Equivalent(_bootstrapEntities, list);
    }

    [Fact]
    public async Task DisablePlugin_NoActionMissingPlugin_Success()
    {
        await _pluginStateController.SaveAsync(_bootstrapEntities, TestContext.Current.CancellationToken);
        var list = await _pluginStateController.LoadFile(TestContext.Current.CancellationToken);
        await _pluginStateController.DisablePlugin("whatever.dfdd", TestContext.Current.CancellationToken);
        Assert.Equivalent(_bootstrapEntities, list);
    }

    private void CleanUp()
    {
        File.Delete(_bootstrapFileLocation);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                CleanUp();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~PluginStateControllerTests()
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

