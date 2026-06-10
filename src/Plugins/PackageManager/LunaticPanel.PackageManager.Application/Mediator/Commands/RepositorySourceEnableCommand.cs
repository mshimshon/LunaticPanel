using LunaticPanel.PackageManager.Application.Payloads;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands;

public sealed record RepositorySourceEnableCommand(RepositorySourcePayload Source)
{
}
