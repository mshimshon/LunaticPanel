using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;

namespace LunaticPanel.Core.Utils.Plugin;

internal partial class PluginLocation : IPluginWebLocation
{
    private const string WEB_RELATIVE_CONTENT_BASE = "/_plugins";
    private const string WEB_RELATIVE_DYNAMIC_BASE = $"{WEB_RELATIVE_CONTENT_BASE}/dynamic";
    private const string WEB_RELATIVE_STATIC_BASE = $"{WEB_RELATIVE_CONTENT_BASE}/static";

    private void InitWebLocation(string assemblyName)
    {


    }
    public string GetRelativeDynamicWebBase() => WEB_RELATIVE_DYNAMIC_BASE + "/" + DotnetAssemblyName;
    public string GetRelativeDynamicWebBase(string[] subFolders)
        => string.Join('/', [GetRelativeDynamicWebBase(), .. subFolders]);
    public string GetRelativeDynamicWebBase(string moduleName)
        => GetRelativeDynamicWebBase([moduleName.ToLower()]);
    public string GetRelativeDynamicWebBase(string moduleName, params string[] subFolders)
         => GetRelativeDynamicWebBase([moduleName.ToLower(), .. subFolders]);
    public string GetRelativeDynamicWebFor(string filename)
                 => GetRelativeDynamicWebBase([filename]);
    public string GetRelativeDynamicWebFor(string[] subFolders, string filename)
        => GetRelativeDynamicWebBase([.. subFolders, filename]);
    public string GetRelativeDynamicWebFor(string moduleName, string filename)
        => GetRelativeDynamicWebBase([moduleName.ToLower(), filename]);
    public string GetRelativeDynamicWebFor(string moduleName, string[] subFolders, string filename)
         => GetRelativeDynamicWebBase([moduleName.ToLower(), .. subFolders, filename]);


    public string GetRelativeStaticWebBase() => WEB_RELATIVE_STATIC_BASE + "/" + DotnetAssemblyName;
    public string GetRelativeStaticWebBase(string[] subFolders)
        => string.Join('/', [GetRelativeStaticWebBase(), .. subFolders]);
    public string GetRelativeStaticWebBase(string moduleName)
        => GetRelativeStaticWebBase([moduleName.ToLower()]);
    public string GetRelativeStaticWebBase(string moduleName, params string[] subFolders)
         => GetRelativeStaticWebBase([moduleName.ToLower(), .. subFolders]);
    public string GetRelativeStaticWebFor(string filename)
                 => GetRelativeStaticWebBase([filename]);
    public string GetRelativeStaticWebFor(string[] subFolders, string filename)
        => GetRelativeStaticWebBase([.. subFolders, filename]);
    public string GetRelativeStaticWebFor(string moduleName, string filename)
        => GetRelativeStaticWebBase([moduleName.ToLower(), filename]);
    public string GetRelativeStaticWebFor(string moduleName, string[] subFolders, string filename)
         => GetRelativeStaticWebBase([moduleName.ToLower(), .. subFolders, filename]);


}
