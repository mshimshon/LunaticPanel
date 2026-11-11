using LunaticPanel.PackageManager.Application.Payloads;
using MaksimShimshon.Mediator;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands;

public sealed record PackageUpdateCommand(PackageManifestPayload Package)
    : IRequest
{
}
