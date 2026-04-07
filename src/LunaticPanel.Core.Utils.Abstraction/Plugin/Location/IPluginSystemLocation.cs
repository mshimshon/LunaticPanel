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
    string GetBashBase(string moduleName);
    string GetBashBase(string moduleName, params string[] subFolders);
    string GetBashFor(string moduleName, string filename);
    string GetBashFor(string moduleName, string[] subFolders, string filename);
    string GetBashFor(string moduleName, string filename, params string[] args);
    string GetBashFor(string moduleName, string[] subFolders, string filename, params string[] args);
}
