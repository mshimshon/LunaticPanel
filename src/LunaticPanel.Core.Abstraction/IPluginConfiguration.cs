namespace LunaticPanel.Core.Abstraction;

public interface IPluginConfiguration
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

    public const string LunaticPanelFolderName = "lunaticpanel";
    public const string LunaticPanelPluginsFolderName = "plugins";
    public const string LinuxUsrFolderName = "usr";
    public const string LinuxLibFolderName = "lib";
    public const string LinuxEtcFolderName = "etc";
    public const string LinuxVarFolderName = "var";

    public string EnsureCreated(string path);
}
