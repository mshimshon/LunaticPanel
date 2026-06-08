using LunaticPanel.PackageManager.Application.Payloads.Enums;

namespace LunaticPanel.PackageManager.Application.Payloads;

public sealed record RepositorySourcePayload
{
    public string Name { get; set; } = default!;
    public string Source { get; set; } = default!;
    public RepositorySourceTypePayload SourceType { get; set; }
}
