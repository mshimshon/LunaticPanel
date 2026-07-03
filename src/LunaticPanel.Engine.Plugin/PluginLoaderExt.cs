using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Engine.Plugin.Entities;
using System.Reflection;

namespace LunaticPanel.Engine.Plugin;

public static class PluginLoaderExt
{
    public static IPlugin CreateEntryPoint(this PluginScannedEntity entity, Assembly? fromAssembly = default)
    {
        var pluginAsm = fromAssembly ?? entity.Loader.Load();
        var entryType = pluginAsm.GetType(entity.PluginEntryLocationType, throwOnError: true)!;
        IPlugin plugin = (IPlugin)Activator.CreateInstance(entryType)!;
        return plugin;
    }
}
