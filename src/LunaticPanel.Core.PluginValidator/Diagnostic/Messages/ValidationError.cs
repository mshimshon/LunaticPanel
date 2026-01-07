namespace LunaticPanel.Core.PluginValidator.Diagnostic.Messages;

public record ValidationError
{
    public string Message { get; init; } = default!;
    public string Origin { get; init; } = default!;

}
