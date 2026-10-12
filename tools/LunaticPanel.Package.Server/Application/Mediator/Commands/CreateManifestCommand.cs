using LunaticPanel.Package.Server.Application.Payloads;
using MaksimShimshon.Mediator;

namespace LunaticPanel.Package.Server.Application.Mediator.Commands;

public sealed record CreateManifestCommand(ManifestPayload Data) : IRequest<ManifestPayload>
{
}
