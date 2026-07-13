using LuncaticPanel.Package.Server.Application.Mediator.Engine;

namespace LuncaticPanel.Package.Server.Application.Mediator.Commands;

public sealed record EndManifestLifeCommand(string Id, string Message) : IRequest
{
}
