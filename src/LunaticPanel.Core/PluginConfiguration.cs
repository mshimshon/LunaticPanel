using LunaticPanel.Core.Abstraction;
using LunaticPanel.Core.Abstraction.Exceptions;
using static LunaticPanel.Core.Abstraction.IPluginLocation;
namespace LunaticPanel.Core;



internal class PluginConfiguration : IPluginLocation
{

    public string PluginFolder { get; }
    public string DotnetAssemblyName { get; }

    public string LinuxAssemblyName { get; }

    public string PluginEtcFolder { get; }

    public string PluginVarFolder { get; }


    private string? GlobalUser { get; set; }
    private string BashFolder { get; }
    private string ReposFolder { get; }
    private string ConfigFolder { get; }


    private string UserDownloadFolderFormat { get; }
    private string UserPluginFolderFormat { get; }
    private string UserConfigFolderFormat { get; }
    private string UserBashFolderFormat { get; }
    private string UserDownloadFolder(string username) => string.Format(UserDownloadFolderFormat, username);
    private string UserConfigFolder(string username) => string.Format(UserConfigFolderFormat, username);
    private string UserBashFolder(string username) => string.Format(UserBashFolderFormat, username);

    public const string BASH_FOLDER_NAME = "bash";
    public const string CONFIG_FOLDER_NAME = "config";
    public const string HOME_FOLDER_NAME = "home";
    public const string REPOS_FOLDER_NAME = "repos";
    public const string DOWNLOAD_FOLDER_NAME = "download";
    public PluginConfiguration(string assemblyName)
    {

        DotnetAssemblyName = assemblyName;
        LinuxAssemblyName = assemblyName.Replace('.', '_').ToLower();
        string pathSep = Path.DirectorySeparatorChar.ToString();
        PluginFolder = EnsureCreated(Path.Combine(pathSep, LinuxUsrFolderName, LinuxLibFolderName, LunaticPanelFolderName, LunaticPanelPluginsFolderName, LinuxAssemblyName));
        PluginEtcFolder = EnsureCreated(Path.Combine(pathSep, LinuxEtcFolderName, LunaticPanelFolderName, LunaticPanelPluginsFolderName, LinuxAssemblyName));
        PluginVarFolder = EnsureCreated(Path.Combine(pathSep, LinuxVarFolderName, LunaticPanelFolderName, LunaticPanelPluginsFolderName, LinuxAssemblyName));

        BashFolder = Path.Combine(PluginFolder, BASH_FOLDER_NAME);
        ConfigFolder = Path.Combine(PluginEtcFolder, CONFIG_FOLDER_NAME);
        ReposFolder = Path.Combine(PluginEtcFolder, REPOS_FOLDER_NAME);

        UserPluginFolderFormat = Path.Combine(pathSep, HOME_FOLDER_NAME, "{0}", LunaticPanelFolderName, LunaticPanelPluginsFolderName, LinuxAssemblyName);
        UserDownloadFolderFormat = Path.Combine(UserPluginFolderFormat, DOWNLOAD_FOLDER_NAME);
        UserConfigFolderFormat = Path.Combine(UserPluginFolderFormat, CONFIG_FOLDER_NAME);
        UserBashFolderFormat = Path.Combine(UserPluginFolderFormat, BASH_FOLDER_NAME);
    }


    public string SetUsername(string username)
        => GlobalUser = username;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public string EnsureCreated(string path)
    {
        var dir = Path.GetDirectoryName(path);
        if (OperatingSystem.IsLinux())
        {
            if (!Directory.Exists(dir))
            {
                Console.WriteLine($"Created (755): {dir}");
                Directory.CreateDirectory(dir!,
                    UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
                    UnixFileMode.GroupRead | UnixFileMode.GroupExecute |
                    UnixFileMode.OtherRead | UnixFileMode.OtherExecute);
            }
        }
        return path;
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


    public string RequiresGlobalUser(string returnPath)
    {
        if (string.IsNullOrWhiteSpace(GlobalUser))
            throw new GlobalUserRequiredException();
        return returnPath;
    }

    public string GetRegionBase(string moduleName, Func<string, string> getBase)
        => RequiresGlobalUser(Path.Combine(getBase(GlobalUser ?? ""), moduleName.ToLower()));
    public string GetRegionBase(string moduleName, string username, Func<string, string> getBase)
        => EnsureCreated(Path.Combine(getBase(username), moduleName.ToLower()));
    public string GetRegionBase(string moduleName, string[] subfolders, Func<string, string> getBase)
        => EnsureCreated(Path.Combine([GetRegionBase(moduleName, getBase), .. subfolders]));
    public string GetRegionBase(string moduleName, string[] subfolders, string username, Func<string, string> getBase)
        => EnsureCreated(Path.Combine([GetRegionBase(moduleName, username, getBase), .. subfolders]));
    public string GetRegionFileFor(string moduleName, string[] subFolders, string filename, Func<string, string> getBase)
        => Path.Combine(GetRegionBase(moduleName, subFolders, getBase), filename);
    public string GetRegionFileFor(string moduleName, string[] subFolders, string filename, string username, Func<string, string> getBase)
        => Path.Combine(GetRegionBase(moduleName, subFolders, username, getBase), filename);
    public string GetRegionFileFor(string moduleName, string filename, Func<string, string> getBase)
        => Path.Combine(GetRegionBase(moduleName, getBase), filename);
    public string GetRegionFileFor(string moduleName, string filename, string username, Func<string, string> getBase)
        => Path.Combine(GetRegionBase(moduleName, username, getBase), filename);


    public string GetUserDownloadBase(string moduleName)
        => GetRegionBase(moduleName, UserDownloadFolder);
    public string GetUserDownloadBase(string moduleName, string username)
        => GetRegionBase(moduleName, username, UserDownloadFolder);
    public string GetUserDownloadBase(string moduleName, params string[] subFolders)
        => GetRegionBase(moduleName, subFolders, UserDownloadFolder);
    public string GetUserDownloadBase(string moduleName, string[] subFolders, string username)
        => GetRegionBase(moduleName, subFolders, username, UserDownloadFolder);
    public string GetUserDownloadFor(string moduleName, string[] subFolders, string filename)
        => GetRegionBase(moduleName, subFolders, filename, UserDownloadFolder);
    public string GetUserDownloadFor(string moduleName, string[] subFolders, string filename, string username)
        => GetRegionFileFor(moduleName, subFolders, filename, username, UserDownloadFolder);
    public string GetUserDownloadFor(string moduleName, string filename)
        => GetRegionBase(moduleName, filename, UserDownloadFolder);
    public string GetUserDownloadFor(string moduleName, string filename, string username)
        => GetRegionFileFor(moduleName, filename, username, UserDownloadFolder);


    public string GetUserConfigBase(string moduleName)
        => GetRegionBase(moduleName, UserConfigFolder);
    public string GetUserConfigBase(string moduleName, string username)
        => GetRegionBase(moduleName, username, UserConfigFolder);
    public string GetUserConfigBase(string moduleName, params string[] subFolders)
        => GetRegionBase(moduleName, subFolders, UserConfigFolder);
    public string GetUserConfigBase(string moduleName, string[] subFolders, string username)
        => GetRegionBase(moduleName, subFolders, username, UserConfigFolder);
    public string GetUserConfigFor(string moduleName, string[] subFolders, string filename)
        => GetRegionFileFor(moduleName, subFolders, filename, UserConfigFolder);
    public string GetUserConfigFor(string moduleName, string[] subFolders, string filename, string username)
        => GetRegionFileFor(moduleName, subFolders, filename, username, UserConfigFolder);
    public string GetUserConfigFor(string moduleName, string filename)
        => GetRegionFileFor(moduleName, filename, UserConfigFolder);
    public string GetUserConfigFor(string moduleName, string filename, string username)
        => GetRegionFileFor(moduleName, filename, username, UserConfigFolder);


    public string ArgumentsToString(params string[] args) => string.Join(' ', args.Select(p => $"\\\"{p}\\\""));
    public string GetUserBashBase(string moduleName)
    => GetRegionBase(moduleName, UserBashFolder);
    public string GetUserBashBase(string moduleName, string username)
        => GetRegionBase(moduleName, username, UserBashFolder);
    public string GetUserBashBase(string moduleName, params string[] subFolders)
        => GetRegionBase(moduleName, subFolders, UserBashFolder);
    public string GetUserBashBase(string moduleName, string[] subFolders, string username)
        => GetRegionBase(moduleName, subFolders, username, UserBashFolder);
    public string GetUserBashFor(string moduleName, string[] subFolders, string filename)
        => GetRegionFileFor(moduleName, subFolders, filename, UserBashFolder);
    public string GetUserBashFor(string moduleName, string[] subFolders, string filename, string username)
        => GetRegionFileFor(moduleName, subFolders, filename, username, UserBashFolder);
    public string GetUserBashFor(string moduleName, string filename)
        => GetRegionFileFor(moduleName, filename, UserBashFolder);
    public string GetUserBashFor(string moduleName, string filename, string username)
        => GetRegionFileFor(moduleName, filename, username, UserBashFolder);
    public string GetUserBashFor(string moduleName, string[] subFolders, string filename, params string[] args)
        => GetRegionFileFor(moduleName, subFolders, filename, UserBashFolder) + " " + ArgumentsToString(args);
    public string GetUserBashFor(string moduleName, string[] subFolders, string filename, string username, params string[] args)
        => GetRegionFileFor(moduleName, subFolders, filename, username, UserBashFolder) + " " + ArgumentsToString(args);
    public string GetUserBashFor(string moduleName, string filename, params string[] args)
        => GetRegionFileFor(moduleName, filename, UserBashFolder) + " " + ArgumentsToString(args);
    public string GetUserBashFor(string moduleName, string filename, string username, params string[] args)
        => GetRegionFileFor(moduleName, filename, username, UserBashFolder) + " " + ArgumentsToString(args);

}