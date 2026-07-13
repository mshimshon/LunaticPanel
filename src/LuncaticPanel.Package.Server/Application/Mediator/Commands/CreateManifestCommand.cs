using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Application.Payloads;

namespace LuncaticPanel.Package.Server.Application.Mediator.Commands;

public sealed record CreateManifestCommand(ManifestPayload Data) : IRequest
{
}
