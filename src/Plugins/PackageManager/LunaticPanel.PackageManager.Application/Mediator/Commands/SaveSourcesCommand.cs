using LunaticPanel.PackageManager.Application.Payloads;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands;

public sealed record SaveSourcesCommand(List<RepositorySourcePayload> Sources) : IRequest
{
}
