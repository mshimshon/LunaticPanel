using LunaticPanel.PackageManager.Application.Payloads;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands;

public sealed record DisableSourceCommand(RepositorySourcePayload Source)
    : IRequest
{
}
