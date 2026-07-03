using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Package.Tool.Exceptions;

internal class PluginDescriptionNotFoundException : HostCodedException
{
    public PluginDescriptionNotFoundException(string pluginId) :
        base(nameof(PluginDescriptionNotFoundException), $"Description for plugin {pluginId} was not found and is required.")
    {
    }
}
