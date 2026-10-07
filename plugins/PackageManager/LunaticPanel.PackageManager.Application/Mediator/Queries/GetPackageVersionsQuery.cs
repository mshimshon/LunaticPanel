using LunaticPanel.PackageManager.Application.Payloads;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries;

public sealed record GetPackageVersionsQuery(string PackageId,
    IEnumerable<RepositorySourcePayload> RepositorySources) : IRequest<ICollection<string>>
{
}
