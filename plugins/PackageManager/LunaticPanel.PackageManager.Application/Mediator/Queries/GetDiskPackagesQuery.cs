using LunaticPanel.PackageManager.Application.Payloads;
using MaksimShimshon.Mediator;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries;

public sealed record GetDiskPackagesQuery : IRequest<DiskPackagesPayload>
{
}
