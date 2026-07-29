using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Engine.Plugin.Exceptions;

public sealed class PluginCoreVersionFailedException : HostCodedException
{
    public PluginCoreVersionFailedException(string message) :
        base(nameof(PluginCoreVersionFailedException), message)
    {
    }
}
