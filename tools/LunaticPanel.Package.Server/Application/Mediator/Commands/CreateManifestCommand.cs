using LunaticPanel.Package.Server.Application.Payloads;
using LunaticPanel.Package.Server.Application.Payloads.Requests;
using MaksimShimshon.Mediator;

namespace LunaticPanel.Package.Server.Application.Mediator.Commands;

public sealed record CreateManifestCommand(PackageValidationRequest Data) : IRequest<ManifestPayload>
{
}
