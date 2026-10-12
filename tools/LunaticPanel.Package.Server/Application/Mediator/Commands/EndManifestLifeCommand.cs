using LunaticPanel.Package.Server.Application.Payloads.Requests;
using MaksimShimshon.Mediator;

namespace LunaticPanel.Package.Server.Application.Mediator.Commands;

public sealed record EndManifestLifeCommand(EndOfLifeRequest Data) : IRequest
{
}
