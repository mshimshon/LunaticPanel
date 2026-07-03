using McMaster.NETCore.Plugins;

namespace LunaticPanel.Package.Tool.Tools.Plugin;

internal static class PluginUtilitiesExt
{
    public static string FindPluginEntryFile(string dir)
    {
        return "";
    }

    public static string GetAssemblyVersion(string dir)
    {
        return "";
    }
    public static PluginLoader PluginLoaderFor(string dll)
    {

        var loader = PluginLoader.CreateFromAssemblyFile(
            dll, new Type[] { }, o =>
            {
                o.LoadInMemory = true;
                o.IsUnloadable = true;
            });
        return loader;
    }
}
