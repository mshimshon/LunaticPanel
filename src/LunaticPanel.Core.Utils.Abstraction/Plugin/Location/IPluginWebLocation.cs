namespace LunaticPanel.Core.Utils.Abstraction.Plugin.Location;

public interface IPluginWebLocation
{
    string GetRelativeStaticWebBase();
    string GetRelativeStaticWebBase(string[] subFolders);
    string GetRelativeStaticWebFor(string filename);
    string GetRelativeStaticWebFor(string[] subFolders, string filename);

    string GetRelativeStaticWebBase(string moduleName);
    string GetRelativeStaticWebBase(string moduleName, params string[] subFolders);
    string GetRelativeStaticWebFor(string moduleName, string filename);
    string GetRelativeStaticWebFor(string moduleName, string[] subFolders, string filename);
    string GetRelativeDynamicWebBase();
    string GetRelativeDynamicWebBase(string[] subFolders);
    string GetRelativeDynamicWebFor(string filename);
    string GetRelativeDynamicWebFor(string[] subFolders, string filename);

    string GetRelativeDynamicWebBase(string moduleName);
    string GetRelativeDynamicWebBase(string moduleName, params string[] subFolders);
    string GetRelativeDynamicWebFor(string moduleName, string filename);
    string GetRelativeDynamicWebFor(string moduleName, string[] subFolders, string filename);
}
