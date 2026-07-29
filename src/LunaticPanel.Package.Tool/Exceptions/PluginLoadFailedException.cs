using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Package.Tool.Exceptions;

public class PluginLoadFailedException : HostCodedException
{
    public PluginLoadFailedException(string dll) :
        base(nameof(PluginLoadFailedException), $"'{dll}' could not load the plugin.")
    {
    }
}
