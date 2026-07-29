using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Core.PluginValidator.Exceptions;

public sealed class PluginEntryViolationException : HostCodedException
{
    public PluginEntryViolationException(string message) :
        base(nameof(PluginEntryViolationException), message)
    {
    }
}
