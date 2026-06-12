using LunaticPanel.PackageManager.Application.Payloads;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands;

public sealed record RepositorySourceAddCommand(RepositorySourcePayload Source) : IRequest
{

}
