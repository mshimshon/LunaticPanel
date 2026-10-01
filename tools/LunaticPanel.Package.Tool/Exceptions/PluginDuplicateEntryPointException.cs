using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Package.Tool.Exceptions;

internal class PluginDuplicateEntryPointException : HostCodedException
{
    public PluginDuplicateEntryPointException() :
        base(nameof(PluginDuplicateEntryPointException), "A plugin cannot have more than on implementation of PluginBase/IPlugin.")
    {
    }
}
