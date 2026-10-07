using LunaticPanel.Package.Server.Application.Payloads.Responses;
using LunaticPanel.Package.Server.Domain.Entites.ValueObjects;

namespace LunaticPanel.Package.Server.Application.Services;

public interface IPackageDownloadResolver
{
    public Task<PackageDownloadTargetResponse> GetDownloadLocationAsync(PackageId packageId, PackageVersion packageVersion, CancellationToken ct = default);
}
