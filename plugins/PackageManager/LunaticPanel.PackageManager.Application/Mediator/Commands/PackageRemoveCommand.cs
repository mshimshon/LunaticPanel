using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands;

public sealed record PackageRemoveCommand(string Id) : IRequest
{
}
