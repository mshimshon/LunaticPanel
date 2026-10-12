using MaksimShimshon.Mediator;

namespace LunaticPanel.Package.Server.Application.Mediator.Commands;

public sealed record HideManifestVersionCommand(string Id, string Version) : IRequest
{
}
