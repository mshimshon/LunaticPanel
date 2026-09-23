using LunaticPanel.PackageManager.Application.Payloads;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries;

public sealed record GetSourcesQuery : IRequest<ICollection<RepositorySourcePayload>>
{
}
