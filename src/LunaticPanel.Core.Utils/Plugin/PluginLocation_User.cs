using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;
using LunaticPanel.Core.Utils.Abstraction.Plugin.Location.Exceptions;

namespace LunaticPanel.Core.Utils.Plugin;

internal partial class PluginLocation : IPluginUserLocation
{
    private string UserDownloadFolderFormat { get; set; } = default!;
    private string UserPluginFolderFormat { get; set; } = default!;
    private string UserConfigFolderFormat { get; set; } = default!;
    private string UserBashFolderFormat { get; set; } = default!;
    private string UserDownloadFolder(string username) => string.Format(UserDownloadFolderFormat, username);
    private string UserConfigFolder(string username) => string.Format(UserConfigFolderFormat, username);
    private string UserBashFolder(string username) => string.Format(UserBashFolderFormat, username);
    private string? GlobalUser { get; set; }
    public const string HOME_FOLDER_NAME = "home";

    private void InitUserLocation(string assemblyName)
    {
        UserPluginFolderFormat = Path.Combine(PathSeparator, HOME_FOLDER_NAME, "{0}", IPluginLocation.LunaticPanelFolderName, IPluginLocation.LunaticPanelPluginsFolderName, LinuxAssemblyName);
        UserDownloadFolderFormat = Path.Combine(UserPluginFolderFormat, DOWNLOAD_FOLDER_NAME);
        UserConfigFolderFormat = Path.Combine(UserPluginFolderFormat, CONFIG_FOLDER_NAME);
        UserBashFolderFormat = Path.Combine(UserPluginFolderFormat, BASH_FOLDER_NAME);
    }

    public string SetUsername(string username)
     => GlobalUser = username;
    public string RequiresGlobalUser(string returnPath)
    {
        if (string.IsNullOrWhiteSpace(GlobalUser))
            throw new GlobalUserRequiredException();
        return returnPath;
    }
    public string GetUserRegionBase(string moduleName, Func<string, string> getBase)
        => RequiresGlobalUser(Path.Combine(getBase(GlobalUser ?? ""), moduleName.ToLower()));
    public string GetUserRegionFileFor(string moduleName, string filename, Func<string, string> getBase)
        => Path.Combine(GetUserRegionBase(moduleName, getBase), filename);
    public string GetUserRegionBase(string moduleName, string[] subfolders, Func<string, string> getBase)
        => EnsureCreated(Path.Combine([GetUserRegionBase(moduleName, getBase), .. subfolders]));
    public string GetUserRegionFileFor(string moduleName, string[] subFolders, string filename, Func<string, string> getBase)
        => Path.Combine(GetUserRegionBase(moduleName, subFolders, getBase), filename);



    public string GetUserConfigBase(string moduleName)
        => GetUserRegionBase(moduleName, UserConfigFolder);
    public string GetUserConfigBase(string moduleName, string username)
        => GetRegionBase(moduleName, username, UserConfigFolder);
    public string GetUserConfigBase(string moduleName, params string[] subFolders)
        => GetUserRegionBase(moduleName, subFolders, UserConfigFolder);
    public string GetUserConfigBase(string moduleName, string[] subFolders, string username)
        => GetRegionBase(moduleName, subFolders, username, UserConfigFolder);
    public string GetUserConfigFor(string moduleName, string[] subFolders, string filename)
        => GetUserRegionFileFor(moduleName, subFolders, filename, UserConfigFolder);
    public string GetUserConfigFor(string moduleName, string[] subFolders, string filename, string username)
        => GetRegionFileFor(moduleName, subFolders, filename, username, UserConfigFolder);
    public string GetUserConfigFor(string moduleName, string filename)
        => GetUserRegionFileFor(moduleName, filename, UserConfigFolder);
    public string GetUserConfigFor(string moduleName, string filename, string username)
        => GetRegionFileFor(moduleName, filename, username, UserConfigFolder);

    public string GetUserDownloadBase(string moduleName)
    => GetUserRegionBase(moduleName, UserDownloadFolder);
    public string GetUserDownloadBase(string moduleName, string username)
        => GetRegionBase(moduleName, username, UserDownloadFolder);
    public string GetUserDownloadBase(string moduleName, params string[] subFolders)
        => GetUserRegionBase(moduleName, subFolders, UserDownloadFolder);
    public string GetUserDownloadBase(string moduleName, string[] subFolders, string username)
        => GetRegionBase(moduleName, subFolders, username, UserDownloadFolder);
    public string GetUserDownloadFor(string moduleName, string[] subFolders, string filename)
        => GetUserRegionFileFor(moduleName, subFolders, filename, UserDownloadFolder);
    public string GetUserDownloadFor(string moduleName, string[] subFolders, string filename, string username)
        => GetRegionFileFor(moduleName, subFolders, filename, username, UserDownloadFolder);
    public string GetUserDownloadFor(string moduleName, string filename)
        => GetUserRegionFileFor(moduleName, filename, UserDownloadFolder);
    public string GetUserDownloadFor(string moduleName, string filename, string username)
        => GetRegionFileFor(moduleName, filename, username, UserDownloadFolder);

    public string GetUserBashBase(string moduleName)
        => GetUserRegionBase(moduleName, UserBashFolder);
    public string GetUserBashBase(string moduleName, string username)
        => GetRegionBase(moduleName, username, UserBashFolder);
    public string GetUserBashBase(string moduleName, params string[] subFolders)
        => GetUserRegionBase(moduleName, subFolders, UserBashFolder);
    public string GetUserBashBase(string moduleName, string[] subFolders, string username)
        => GetRegionBase(moduleName, subFolders, username, UserBashFolder);
    public string GetUserBashFor(string moduleName, string[] subFolders, string filename)
        => GetUserRegionFileFor(moduleName, subFolders, filename, UserBashFolder);
    public string GetUserBashFor(string moduleName, string[] subFolders, string filename, string username)
        => GetRegionFileFor(moduleName, subFolders, filename, username, UserBashFolder);
    public string GetUserBashFor(string moduleName, string filename)
        => GetUserRegionFileFor(moduleName, filename, UserBashFolder);
    public string GetUserBashFor(string moduleName, string filename, string username)
        => GetRegionFileFor(moduleName, filename, username, UserBashFolder);
    public string GetUserBashFor(string moduleName, string[] subFolders, string filename, params string[] args)
        => GetUserRegionFileFor(moduleName, subFolders, filename, UserBashFolder) + " " + ArgumentsToString(args);
    public string GetUserBashFor(string moduleName, string[] subFolders, string filename, string username, params string[] args)
        => GetRegionFileFor(moduleName, subFolders, filename, username, UserBashFolder) + " " + ArgumentsToString(args);
    public string GetUserBashFor(string moduleName, string filename, params string[] args)
        => GetUserRegionFileFor(moduleName, filename, UserBashFolder) + " " + ArgumentsToString(args);
    public string GetUserBashFor(string moduleName, string filename, string username, params string[] args)
        => GetRegionFileFor(moduleName, filename, username, UserBashFolder) + " " + ArgumentsToString(args);
}
