using LunaticPanel.PackageManager.Application.Payloads;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries;

public sealed record GetPackagesLatestVersionQuery(IEnumerable<string> Packages,
    IEnumerable<RepositorySourcePayload> RepositorySources)
    : IRequest<ICollection<PackagePayload>>
{
}
