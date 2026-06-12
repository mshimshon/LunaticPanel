using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands;

public sealed record PackageDisableCommand(string Id) : IRequest
{
}
