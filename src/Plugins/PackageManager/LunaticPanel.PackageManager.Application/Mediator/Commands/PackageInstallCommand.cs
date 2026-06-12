using LunaticPanel.PackageManager.Application.Payloads;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands;

public sealed record PackageInstallCommand(string Id, string Version, RepositorySourcePayload Source)
    : IRequest
{
}
