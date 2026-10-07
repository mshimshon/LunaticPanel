using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;

namespace LunaticPanel.Core.Utils.Plugin;

internal partial class PluginLocation : IPluginSystemLocation
{
    public string PluginFolder { get; set; } = default!;
    public string PluginEtcFolder { get; set; } = default!;
    public string PluginVarFolder { get; set; } = default!;
    private string BashFolder { get; set; } = default!;
    private string DownloadFolder { get; set; } = default!;
    private string StaticWebContentFolder { get; set; } = default!;
    private string DynamicWebContentFolder { get; set; } = default!;

    private string ReposFolder { get; set; } = default!;
    private string ConfigFolder { get; set; } = default!;


    public const string BASH_FOLDER_NAME = "bash";
    public const string WEB_FOLDER_NAME = "wwwroot";
    public const string CONFIG_FOLDER_NAME = "config";
    public const string REPOS_FOLDER_NAME = "repos";
    public const string DOWNLOAD_FOLDER_NAME = "download";
    private void InitSystemLocation(string assemblyName)
    {
        PluginFolder = EnsureCreated(Path.Combine(PathSeparator, IPluginLocation.LinuxSrvFolderName, IPluginLocation.LunaticPanelFolderName, IPluginLocation.LunaticPanelPluginsFolderName, LinuxAssemblyName));
        PluginEtcFolder = EnsureCreated(Path.Combine(PathSeparator, IPluginLocation.LinuxEtcFolderName, IPluginLocation.LunaticPanelFolderName, IPluginLocation.LunaticPanelPluginsFolderName, LinuxAssemblyName));
        PluginVarFolder = EnsureCreated(Path.Combine(PathSeparator, IPluginLocation.LinuxVarFolderName, IPluginLocation.LinuxLibFolderName, IPluginLocation.LunaticPanelFolderName, IPluginLocation.LunaticPanelPluginsFolderName, LinuxAssemblyName));
        StaticWebContentFolder = Path.Combine(PluginFolder, WEB_FOLDER_NAME);
        DynamicWebContentFolder = Path.Combine(PluginVarFolder, WEB_FOLDER_NAME);
        BashFolder = GetAppBinBase(BASH_FOLDER_NAME);
        ConfigFolder = GetAppConfigBase(CONFIG_FOLDER_NAME);
        ReposFolder = GetAppDataBase(REPOS_FOLDER_NAME);
        DownloadFolder = GetAppDataBase(PluginVarFolder, DOWNLOAD_FOLDER_NAME);
    }

    public string GetReposBase(string moduleName)
    => EnsureCreated(Path.Combine(ReposFolder, moduleName.ToLower()));
    public string GetReposBase(string moduleName, params string[] subFolders)
        => EnsureCreated(Path.Combine([GetReposBase(moduleName), .. subFolders]));
    public string GetReposFor(string moduleName, string repos)
        => EnsureCreated(Path.Combine(GetReposBase(moduleName), repos));
    public string GetReposFor(string moduleName, string[] subFolders, string repos)
        => EnsureCreated(Path.Combine(GetReposBase(moduleName, subFolders), repos));

    public string GetConfigBase(string moduleName)
        => EnsureCreated(Path.Combine(ConfigFolder, moduleName.ToLower()));
    public string GetConfigBase(string moduleName, params string[] subFolders)
        => EnsureCreated(Path.Combine([GetConfigBase(moduleName), .. subFolders]));
    public string GetConfigFor(string moduleName, string filename)
    => Path.Combine(GetConfigBase(moduleName), filename);

    public string GetConfigFor(string moduleName, string[] subFolders, string filename)
        => Path.Combine(GetConfigBase(moduleName, subFolders), filename);

    public string GetBashBase(string moduleName)
        => EnsureCreated(Path.Combine(BashFolder, moduleName.ToLower()));
    public string GetBashBase(string moduleName, params string[] subFolders)
        => EnsureCreated(Path.Combine([GetBashBase(moduleName), .. subFolders]));
    public string GetBashFor(string moduleName, string filename)
        => Path.Combine(GetBashBase(moduleName), filename);
    public string GetBashFor(string moduleName, string[] subFolders, string filename)
    => Path.Combine(GetBashBase(moduleName, subFolders), filename);
    public string GetBashFor(string moduleName, string filename, params string[] args)
        => GetBashFor(moduleName, filename) + " " + ArgumentsToString(args);
    public string GetBashFor(string moduleName, string[] subFolders, string filename, params string[] args)
        => GetBashFor(moduleName, subFolders, filename) + " " + ArgumentsToString(args);

    public string GetAppBinBase()
    => EnsureCreated(PluginFolder);
    public string GetAppBinBase(params string[] subFolders)
        => EnsureCreated(Path.Combine([GetAppBinBase(), .. subFolders]));

    public string GetAppConfigBase()
        => EnsureCreated(PluginEtcFolder);
    public string GetAppConfigBase(params string[] subFolders)
        => EnsureCreated(Path.Combine([GetAppConfigBase(), .. subFolders]));

    public string GetAppDataBase()
    => EnsureCreated(PluginVarFolder);
    public string GetAppDataBase(params string[] subFolders)
        => EnsureCreated(Path.Combine([GetAppDataBase(), .. subFolders]));

    public string GetDownloadBase(string moduleName)
        => EnsureCreated(GetAppDataBase(DownloadFolder, moduleName.ToLower()));
    public string GetDownloadBase(string moduleName, params string[] subFolders)
        => EnsureCreated(Path.Combine([GetDownloadBase(moduleName), .. subFolders]));
    public string GetDownloadFor(string moduleName, string[] subFolders, string filename)
        => Path.Combine(GetDownloadBase(moduleName, subFolders), filename);
    public string GetDownloadFor(string moduleName, string filename)
          => Path.Combine(GetDownloadBase(moduleName), filename);

    public string GetStaticWebContentBase() => EnsureCreated(StaticWebContentFolder);
    public string GetStaticWebContentBase(string[] subFolders)
         => EnsureCreated(Path.Combine([GetStaticWebContentBase(), .. subFolders]));
    public string GetStaticWebContentFor(string filename)
        => Path.Combine(GetStaticWebContentBase(), filename);
    public string GetStaticWebContentFor(string[] subFolders, string filename)
        => Path.Combine(GetStaticWebContentBase(subFolders), filename);

    public string GetStaticWebContentBase(string moduleName)
        => GetStaticWebContentBase([moduleName.ToLower()]);
    public string GetStaticWebContentBase(string moduleName, params string[] subFolders)
        => GetStaticWebContentBase([moduleName.ToLower(), .. subFolders]);
    public string GetStaticWebContentFor(string moduleName, string filename)
        => GetStaticWebContentFor([moduleName.ToLower()], filename);
    public string GetStaticWebContentFor(string moduleName, string[] subFolders, string filename)
        => GetStaticWebContentFor([moduleName.ToLower(), .. subFolders], filename);

    public string GetDynamicWebContentBase() => EnsureCreated(DynamicWebContentFolder);
    public string GetDynamicWebContentBase(string[] subFolders)
        => EnsureCreated(Path.Combine([GetDynamicWebContentBase(), .. subFolders]));
    public string GetDynamicWebContentFor(string filename) => Path.Combine(GetDynamicWebContentBase(), filename);
    public string GetDynamicWebContentFor(string[] subFolders, string filename)
        => Path.Combine(GetDynamicWebContentBase(subFolders), filename);

    public string GetDynamicWebContentBase(string moduleName)
        => GetDynamicWebContentBase([moduleName.ToLower()]);
    public string GetDynamicWebContentBase(string moduleName, params string[] subFolders)
        => GetDynamicWebContentBase([moduleName.ToLower(), .. subFolders]);
    public string GetDynamicWebContentFor(string moduleName, string filename)
        => GetDynamicWebContentFor([moduleName.ToLower()], filename);
    public string GetDynamicWebContentFor(string moduleName, string[] subFolders, string filename)
        => GetDynamicWebContentFor([moduleName.ToLower(), .. subFolders], filename);

}
