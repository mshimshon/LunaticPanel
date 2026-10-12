using MaksimShimshon.Mediator;

namespace LunaticPanel.Package.Server.Application.Mediator.Commands;

public sealed record PackageUploadCommand(string UploadId) : IRequest
{
}
