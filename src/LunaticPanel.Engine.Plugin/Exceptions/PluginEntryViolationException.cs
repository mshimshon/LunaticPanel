using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Engine.Plugin.Exceptions;

public sealed class PluginEntryViolationException : HostCodedException
{
    public PluginEntryViolationException(string message) :
        base(nameof(PluginEntryViolationException), message)
    {
    }
}
