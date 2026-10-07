namespace LunaticPanel.Core.Utils.Abstraction.Plugin.Location;

public interface IPluginSystemLocation
{
    string GetReposBase(string moduleName);
    string GetReposBase(string moduleName, params string[] subFolders);
    string GetReposFor(string moduleName, string repos);
    string GetReposFor(string moduleName, string[] subFolders, string repos);
    string GetConfigBase(string moduleName);
    string GetConfigBase(string moduleName, params string[] subFolders);
    string GetConfigFor(string moduleName, string filename);
    string GetConfigFor(string moduleName, string[] subFolders, string filename);
    string GetAppConfigBase();
    string GetAppConfigBase(params string[] subFolders);
    string GetAppDataBase();
    string GetAppDataBase(params string[] subFolders);
    public string GetAppBinBase();
    public string GetAppBinBase(params string[] subFolders);
    string GetDownloadBase(string moduleName);
    string GetDownloadBase(string moduleName, params string[] subFolders);
    string GetDownloadFor(string moduleName, string[] subFolders, string filename);
    string GetDownloadFor(string moduleName, string filename);


    string GetStaticWebContentBase();
    string GetStaticWebContentBase(string[] subFolders);
    string GetStaticWebContentFor(string filename);
    string GetStaticWebContentFor(string[] subFolders, string filename);

    string GetStaticWebContentBase(string moduleName);
    string GetStaticWebContentBase(string moduleName, params string[] subFolders);
    string GetStaticWebContentFor(string moduleName, string filename);
    string GetStaticWebContentFor(string moduleName, string[] subFolders, string filename);
    string GetDynamicWebContentBase();
    string GetDynamicWebContentBase(string[] subFolders);
    string GetDynamicWebContentFor(string filename);
    string GetDynamicWebContentFor(string[] subFolders, string filename);

    string GetDynamicWebContentBase(string moduleName);
    string GetDynamicWebContentBase(string moduleName, params string[] subFolders);
    string GetDynamicWebContentFor(string moduleName, string filename);
    string GetDynamicWebContentFor(string moduleName, string[] subFolders, string filename);

    string GetBashBase(string moduleName);
    string GetBashBase(string moduleName, params string[] subFolders);
    string GetBashFor(string moduleName, string filename);
    string GetBashFor(string moduleName, string[] subFolders, string filename);
    string GetBashFor(string moduleName, string filename, params string[] args);
    string GetBashFor(string moduleName, string[] subFolders, string filename, params string[] args);
}
