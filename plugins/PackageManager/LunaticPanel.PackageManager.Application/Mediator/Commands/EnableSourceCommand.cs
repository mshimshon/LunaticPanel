using LunaticPanel.PackageManager.Application.Payloads;
using MaksimShimshon.Mediator;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands;

public sealed record EnableSourceCommand(RepositorySourcePayload Source)
    : IRequest
{
}
