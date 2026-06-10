using LunaticPanel.PackageManager.Application.Payloads;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries;

internal sealed record SearchRepositoryQuery
{
    public List<RepositorySourcePayload> Sources { get; set; } = new();
    public string Keywords { get; set; } = string.Empty;

}
