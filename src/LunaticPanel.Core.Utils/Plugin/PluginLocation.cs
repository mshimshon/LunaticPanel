using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;
namespace LunaticPanel.Core.Utils.Plugin;

internal partial class PluginLocation : IPluginLocation
{
    public string DotnetAssemblyName { get; }

    public string LinuxAssemblyName { get; }

    private string PathSeparator { get; } = Path.DirectorySeparatorChar.ToString();
    public PluginLocation(string assemblyName)
    {
        DotnetAssemblyName = assemblyName;
        LinuxAssemblyName = assemblyName.Replace('.', '_').ToLower();
        InitUserLocation(assemblyName);
        InitWebLocation(assemblyName);
        InitSystemLocation(assemblyName);
    }


    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public string EnsureCreated(string path)
    {
        var dir = Path.GetDirectoryName(path);
        if (OperatingSystem.IsLinux())
        {
            if (!Directory.Exists(path))
            {
                Console.Out.WriteLine($"Created (755): {dir}");
                Directory.CreateDirectory(path,
                    UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
                    UnixFileMode.GroupRead | UnixFileMode.GroupExecute |
                    UnixFileMode.OtherRead | UnixFileMode.OtherExecute);
            }
        }
        return path;
    }

    public string GetRegionBase(string moduleName, string username, Func<string, string> getBase)
    => EnsureCreated(Path.Combine(getBase(username), moduleName.ToLower()));

    public string GetRegionBase(string moduleName, string[] subfolders, string username, Func<string, string> getBase)
        => EnsureCreated(Path.Combine([GetRegionBase(moduleName, username, getBase), .. subfolders]));

    public string GetRegionFileFor(string moduleName, string[] subFolders, string filename, string username, Func<string, string> getBase)
        => Path.Combine(GetRegionBase(moduleName, subFolders, username, getBase), filename);

    public string GetRegionFileFor(string moduleName, string filename, string username, Func<string, string> getBase)
        => Path.Combine(GetRegionBase(moduleName, username, getBase), filename);
    public string ArgumentsToString(params string[] args) => string.Join(' ', args.Select(p => $"\\\"{p}\\\""));

}