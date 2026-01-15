using LunaticPanel.Core.Abstraction;

using static LunaticPanel.Core.Abstraction.IPluginConfiguration;
namespace LunaticPanel.Core;

internal class PluginConfiguration : IPluginConfiguration
{
    public string PluginFolder { get; }
    public string DotnetAssemblyName { get; }

    public string LinuxAssemblyName { get; }

    public PluginConfiguration(string assemblyName)
    {

        DotnetAssemblyName = assemblyName;
        LinuxAssemblyName = assemblyName.Replace('.', '_').ToLower();
        PluginFolder = Path.Combine("/", LinuxUsrFolderName, LinuxLibFolderName, LunaticPanelFolderName, LunaticPanelPluginsFolderName, LinuxAssemblyName);
    }
}