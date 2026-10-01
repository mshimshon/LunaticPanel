using LunaticPanel.PackageManager.Application.Payloads;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands;

public sealed record MoveUpSourceCommand(RepositorySourcePayload Source)
    : IRequest
{
}
