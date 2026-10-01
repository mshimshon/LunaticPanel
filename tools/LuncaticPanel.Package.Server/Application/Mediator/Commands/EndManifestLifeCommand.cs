using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Application.Payloads.Requests;

namespace LuncaticPanel.Package.Server.Application.Mediator.Commands;

public sealed record EndManifestLifeCommand(EndOfLifeRequest Data) : IRequest
{
}
