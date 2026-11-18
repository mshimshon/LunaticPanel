namespace LunaticPanel.Core.Utils.Abstraction.Plugin.Location;
// TODO: REMOVE IPluginSystemLocation, IPluginUserLocation, IPluginWebLocation
public interface IPluginLocation : IPluginSystemLocation, IPluginUserLocation, IPluginWebLocation
{
    /// <summary>
    /// This is the Dotnet version MyAssembly.Namespace
    /// </summary>
    string DotnetAssemblyName { get; }
    /// <summary>
    /// This is the Linux modified myassembly_namespace as folder name.
    /// </summary>
    string LinuxAssemblyName { get; }

    /// <summary>
    /// This is the plugin's main folder.
    /// </summary>
    string PluginFolder { get; }
    /// <summary>
    /// Plugin Configurations
    /// </summary>
    string PluginEtcFolder { get; }
    /// <summary>
    /// Plugin Var folder
    /// </summary>
    string PluginVarFolder { get; }

    const string LUNATIC_PANEL_FOLDER_NAME = "lunaticpanel";
    const string LUNATIC_PANEL_PLUGINS_CACHE_FOLDER_NAME = ".plugins";
    const string LUNATIC_PANEL_PLUGINS_FOLDER_NAME = "plugins";
    const string LUNATIC_PANEL_PLUGINS_CACHE_RUNTIME_FOLDER_NAME = "runtime";
    const string LINUX_USR_FOLDER_NAME = "usr";
    const string LINUX_SRV_FOLDER_NAME = "srv";
    const string LINUX_LIB_FOLDER_NAME = "lib";
    const string LINUX_ETC_FOLDER_NAME = "etc";
    const string LINUX_VAR_FOLDER_NAME = "var";

    string EnsureCreated(string path);
    string ArgumentsToString(params string[] args);
    string GetUserRegionBase(string moduleName, Func<string, string> getBase);
    string GetRegionBase(string moduleName, string username, Func<string, string> getBase);
    string GetUserRegionBase(string moduleName, string[] subfolders, Func<string, string> getBase);
    string GetRegionBase(string moduleName, string[] subfolders, string username, Func<string, string> getBase);
    string GetUserRegionFileFor(string moduleName, string[] subFolders, string filename, Func<string, string> getBase);
    string GetRegionFileFor(string moduleName, string[] subFolders, string filename, string username, Func<string, string> getBase);
    string GetUserRegionFileFor(string moduleName, string filename, Func<string, string> getBase);
    string GetRegionFileFor(string moduleName, string filename, string username, Func<string, string> getBase);
}
