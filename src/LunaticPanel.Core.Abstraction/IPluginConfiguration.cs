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
    public string GetReposBase(string moduleName);
    public string GetReposBase(string moduleName, params string[] subFolders);
    public string GetReposFor(string moduleName, string repos);
    public string GetReposFor(string moduleName, string[] subFolders, string repos);
    public string GetConfigBase(string moduleName);
    public string GetConfigBase(string moduleName, params string[] subFolders);
    public string GetConfigFor(string moduleName, string filename);
    public string GetConfigFor(string moduleName, string[] subFolders, string filename);
    public string GetBashBase(string moduleName);
    public string GetBashBase(string moduleName, params string[] subFolders);
    public string GetBashFor(string moduleName, string filename);
    public string GetBashFor(string moduleName, string[] subFolders, string filename);
    public string GetBashFor(string moduleName, string filename, params string[] args);
    public string GetBashFor(string moduleName, string[] subFolders, string filename, params string[] args);
    public string ArgumentsToString(params string[] args);

    public string SetUsername(string username);
    public string GetRegionBase(string moduleName, Func<string, string> getBase);
    public string GetRegionBase(string moduleName, string username, Func<string, string> getBase);
    public string GetRegionBase(string moduleName, string[] subfolders, Func<string, string> getBase);
    public string GetRegionBase(string moduleName, string[] subfolders, string username, Func<string, string> getBase);
    public string GetRegionFileFor(string moduleName, string[] subFolders, string filename, Func<string, string> getBase);
    public string GetRegionFileFor(string moduleName, string[] subFolders, string filename, string username, Func<string, string> getBase);
    public string GetRegionFileFor(string moduleName, string filename, Func<string, string> getBase);
    public string GetRegionFileFor(string moduleName, string filename, string username, Func<string, string> getBase);
    public string GetUserDownloadBase(string moduleName);
    public string GetUserDownloadBase(string moduleName, string username);
    public string GetUserDownloadBase(string moduleName, params string[] subFolders);
    public string GetUserDownloadBase(string moduleName, string[] subFolders, string username);
    public string GetUserDownloadFor(string moduleName, string[] subFolders, string filename);
    public string GetUserDownloadFor(string moduleName, string[] subFolders, string filename, string username);
    public string GetUserDownloadFor(string moduleName, string filename);
    public string GetUserDownloadFor(string moduleName, string filename, string username);


    public string GetUserConfigBase(string moduleName);
    public string GetUserConfigBase(string moduleName, string username);
    public string GetUserConfigBase(string moduleName, params string[] subFolders);
    public string GetUserConfigBase(string moduleName, string[] subFolders, string username);
    public string GetUserConfigFor(string moduleName, string[] subFolders, string filename);
    public string GetUserConfigFor(string moduleName, string[] subFolders, string filename, string username);
    public string GetUserConfigFor(string moduleName, string filename);
    public string GetUserConfigFor(string moduleName, string filename, string username);


    public string GetUserBashBase(string moduleName);
    public string GetUserBashBase(string moduleName, string username);
    public string GetUserBashBase(string moduleName, params string[] subFolders);
    public string GetUserBashBase(string moduleName, string[] subFolders, string username);
    public string GetUserBashFor(string moduleName, string[] subFolders, string filename);
    public string GetUserBashFor(string moduleName, string[] subFolders, string filename, string username);
    public string GetUserBashFor(string moduleName, string filename);
    public string GetUserBashFor(string moduleName, string filename, string username);
    public string GetUserBashFor(string moduleName, string[] subFolders, string filename, params string[] args);
    public string GetUserBashFor(string moduleName, string[] subFolders, string filename, string username, params string[] args);
    public string GetUserBashFor(string moduleName, string filename, params string[] args);
    public string GetUserBashFor(string moduleName, string filename, string username, params string[] args);

}
