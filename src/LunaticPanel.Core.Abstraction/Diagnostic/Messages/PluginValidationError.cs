namespace LunaticPanel.Core.Abstraction.Diagnostic.Messages;

public record PluginValidationError
{
    public string Message { get; init; } = default!;
    public string Origin { get; init; } = default!;

}
