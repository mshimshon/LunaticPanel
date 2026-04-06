using LunaticPanel.Core.Abstraction.Plugin.Configuration;

namespace LunaticPanel.Core.Abstraction;

public interface IPluginLocation : IPluginSystemLocation, IPluginUserLocation
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

    const string LunaticPanelFolderName = "lunaticpanel";
    const string LunaticPanelPluginsFolderName = "plugins";
    const string LinuxUsrFolderName = "usr";
    const string LinuxLibFolderName = "lib";
    const string LinuxEtcFolderName = "etc";
    const string LinuxVarFolderName = "var";

    string EnsureCreated(string path);
    string ArgumentsToString(params string[] args);
    string GetRegionBase(string moduleName, Func<string, string> getBase);
    string GetRegionBase(string moduleName, string username, Func<string, string> getBase);
    string GetRegionBase(string moduleName, string[] subfolders, Func<string, string> getBase);
    string GetRegionBase(string moduleName, string[] subfolders, string username, Func<string, string> getBase);
    string GetRegionFileFor(string moduleName, string[] subFolders, string filename, Func<string, string> getBase);
    string GetRegionFileFor(string moduleName, string[] subFolders, string filename, string username, Func<string, string> getBase);
    string GetRegionFileFor(string moduleName, string filename, Func<string, string> getBase);
    string GetRegionFileFor(string moduleName, string filename, string username, Func<string, string> getBase);
}
