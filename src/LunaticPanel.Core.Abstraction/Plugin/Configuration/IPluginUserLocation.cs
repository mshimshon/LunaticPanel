namespace LunaticPanel.Core.Abstraction.Plugin.Configuration;

public interface IPluginUserLocation
{
    string SetUsername(string username);

    string GetUserDownloadBase(string moduleName);
    string GetUserDownloadBase(string moduleName, string username);
    string GetUserDownloadBase(string moduleName, params string[] subFolders);
    string GetUserDownloadBase(string moduleName, string[] subFolders, string username);
    string GetUserDownloadFor(string moduleName, string[] subFolders, string filename);
    string GetUserDownloadFor(string moduleName, string[] subFolders, string filename, string username);
    string GetUserDownloadFor(string moduleName, string filename);
    string GetUserDownloadFor(string moduleName, string filename, string username);


    string GetUserConfigBase(string moduleName);
    string GetUserConfigBase(string moduleName, string username);
    string GetUserConfigBase(string moduleName, params string[] subFolders);
    string GetUserConfigBase(string moduleName, string[] subFolders, string username);
    string GetUserConfigFor(string moduleName, string[] subFolders, string filename);
    string GetUserConfigFor(string moduleName, string[] subFolders, string filename, string username);
    string GetUserConfigFor(string moduleName, string filename);
    string GetUserConfigFor(string moduleName, string filename, string username);


    string GetUserBashBase(string moduleName);
    string GetUserBashBase(string moduleName, string username);
    string GetUserBashBase(string moduleName, params string[] subFolders);
    string GetUserBashBase(string moduleName, string[] subFolders, string username);
    string GetUserBashFor(string moduleName, string[] subFolders, string filename);
    string GetUserBashFor(string moduleName, string[] subFolders, string filename, string username);
    string GetUserBashFor(string moduleName, string filename);
    string GetUserBashFor(string moduleName, string filename, string username);
    string GetUserBashFor(string moduleName, string[] subFolders, string filename, params string[] args);
    string GetUserBashFor(string moduleName, string[] subFolders, string filename, string username, params string[] args);
    string GetUserBashFor(string moduleName, string filename, params string[] args);
    string GetUserBashFor(string moduleName, string filename, string username, params string[] args);
}
