using LunaticPanel.Core.Abstraction;

using static LunaticPanel.Core.Abstraction.IPluginConfiguration;
namespace LunaticPanel.Core;

internal class PluginConfiguration : IPluginConfiguration
{
    public string PluginFolder { get; }
    public string DotnetAssemblyName { get; }

    public string LinuxAssemblyName { get; }

    public string PluginEtcFolder { get; }

    public string PluginVarFolder { get; }

    public PluginConfiguration(string assemblyName)
    {

        DotnetAssemblyName = assemblyName;
        LinuxAssemblyName = assemblyName.Replace('.', '_').ToLower();
        PluginFolder = EnsureCreated(Path.Combine("/", LinuxUsrFolderName, LinuxLibFolderName, LunaticPanelFolderName, LunaticPanelPluginsFolderName, LinuxAssemblyName));
        PluginEtcFolder = EnsureCreated(Path.Combine("/", LinuxEtcFolderName, LunaticPanelFolderName, LunaticPanelPluginsFolderName, LinuxAssemblyName));
        PluginVarFolder = EnsureCreated(Path.Combine("/", LinuxVarFolderName, LunaticPanelFolderName, LunaticPanelPluginsFolderName, LinuxAssemblyName));

    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public string EnsureCreated(string path)
    {
        var dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Console.WriteLine($"Created (755): {dir}");
            Directory.CreateDirectory(dir!,
                UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute |
                UnixFileMode.GroupRead | UnixFileMode.GroupExecute |
                UnixFileMode.OtherRead | UnixFileMode.OtherExecute);
        }
        return path;
    }
}