using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads.Enums;

namespace LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads;

internal sealed record ExternalSourceRepositoryPayload
{
    public string Name { get; set; } = default!;
    public string Source { get; set; } = default!;
    public ExternalSourceRepositoryTypePayload SourceType { get; set; }

    public ExternalSourceRepositoryStatePayload State { get; set; }
}
