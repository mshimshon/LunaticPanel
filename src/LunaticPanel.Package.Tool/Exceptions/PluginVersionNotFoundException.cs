using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Package.Tool.Exceptions;

internal class PluginVersionNotFoundException : HostCodedException
{
    public PluginVersionNotFoundException(string pluginId) :
        base(nameof(PluginVersionNotFoundException), $"Version was not found for plugin {pluginId}.")
    {
    }
}
