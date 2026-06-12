using LunaticPanel.PackageManager.Application.Payloads;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands;

public sealed record PackageUpdateCommand(string Id, string FromVersion, string ToVersion, RepositorySourcePayload Source)
    : IRequest
{
}
