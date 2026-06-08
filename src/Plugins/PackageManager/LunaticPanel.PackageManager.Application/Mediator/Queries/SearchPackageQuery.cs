using LunaticPanel.PackageManager.Application.Payloads;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries;

public sealed record SearchPackageQuery
{
    public List<RepositorySourcePayload> InsideSources { get; set; } = new List<RepositorySourcePayload>();
    public string Keywords { get; set; } = string.Empty;
}
