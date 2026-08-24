using LunaticPanel.PackageManager.Application.Payloads.Enums;

namespace LunaticPanel.PackageManager.Application.Payloads;

public sealed record RepositorySourcePayload
{
    public string Name { get; init; } = default!;
    public string Source { get; init; } = default!;
    public RepositorySourceTypePayload SourceType { get; init; }

    public RepositorySourceStatePayload State { get; init; }
    public string? Failure { get; init; }
}
