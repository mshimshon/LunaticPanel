namespace LunaticPanel.Core.PluginValidator.Diagnostic.Messages;

public record ValidationResult
{
    public IReadOnlyCollection<ValidationError>? Errors { get; init; }
    public bool Passed => Errors == default || Errors.Count <= 0;
    public string PluginId { get; init; } = default;
}
