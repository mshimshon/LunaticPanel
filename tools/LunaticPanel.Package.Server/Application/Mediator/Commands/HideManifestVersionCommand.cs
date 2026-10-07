using LunaticPanel.Package.Server.Application.Mediator.Engine;

namespace LunaticPanel.Package.Server.Application.Mediator.Commands;

public sealed record HideManifestVersionCommand(string Id, string Version) : IRequest
{
}
