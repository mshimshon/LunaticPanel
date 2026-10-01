using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Package.Tool.Exceptions;

internal class PluginHardDependencyViolationException : HostCodedException
{
    public PluginHardDependencyViolationException(string location) :
        base(nameof(PluginHardDependencyViolationException), $"Plugin Folder {location} contains dll from more than one plugin... a plugin CANNOT depend directly on other plugins you must use bus system with graceful degradation pattern.")
    {
    }
}
