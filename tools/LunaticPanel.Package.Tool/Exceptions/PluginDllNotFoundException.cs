using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Package.Tool.Exceptions;

internal class PluginDllNotFoundException : HostCodedException
{
    public PluginDllNotFoundException(string pluginDir) :
        base(nameof(PluginDllNotFoundException), $"{pluginDir} does not contain plugin DLL or IPlugin is not located at the Root namespace.")
    {
    }
}
