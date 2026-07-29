using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Core.PluginValidator.Exceptions;

public sealed class PluginCoreVersionFailedException : HostCodedException
{
    public PluginCoreVersionFailedException(string message) :
        base(nameof(PluginCoreVersionFailedException), message)
    {
    }
}
