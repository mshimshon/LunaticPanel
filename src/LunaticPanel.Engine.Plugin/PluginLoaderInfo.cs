using McMaster.NETCore.Plugins;
using System.Reflection;

namespace LunaticPanel.Engine.Plugin;

internal class PluginLoaderInfo : IPluginLoader
{
    private readonly PluginLoader _pluginLoader;
    private Assembly? _loaded = default;
    public PluginLoaderInfo(PluginLoader pluginLoader)
    {
        _pluginLoader = pluginLoader;
    }
    public Assembly Load()
    {
        if (_loaded != default)
            return _loaded;
        _loaded = _pluginLoader.LoadDefaultAssembly();
        return _loaded;
    }
    public void Unload()
    {
        _pluginLoader.Dispose();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
}
