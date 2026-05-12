namespace LunaticPanel.Core.Abstraction.Diagnostic.Messages;

public record PluginValidationResult
{
    public IReadOnlyCollection<PluginValidationError>? Errors { get; init; }
    public bool Passed => Errors == default || Errors.Count <= 0;
    public string PluginId { get; init; } = default;
}
