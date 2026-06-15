using LunaticPanel.PackageManager.Application.Payloads;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands;

public sealed record PackageInstallCommand(PackagePayload Data, RepositorySourcePayload Source)
    : IRequest
{
}
