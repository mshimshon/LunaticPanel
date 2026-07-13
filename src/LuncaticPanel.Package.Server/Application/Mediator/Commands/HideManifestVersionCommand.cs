using LuncaticPanel.Package.Server.Application.Mediator.Engine;

namespace LuncaticPanel.Package.Server.Application.Mediator.Commands;

public sealed record HideManifestVersionCommand(string Id, string Version) : IRequest
{
}
