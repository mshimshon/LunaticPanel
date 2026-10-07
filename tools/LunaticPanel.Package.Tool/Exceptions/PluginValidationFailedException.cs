using LunaticPanel.Core.Abstraction.Diagnostic.Messages;
using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Package.Tool.Exceptions;

internal class PluginValidationFailedException : HostCodedException
{
    public PluginValidationFailedException(string pluginId, PluginValidationError error) :
        base(nameof(PluginValidationFailedException), $"{pluginId} reported a validation failure {error.Message}")
    {
    }
}
