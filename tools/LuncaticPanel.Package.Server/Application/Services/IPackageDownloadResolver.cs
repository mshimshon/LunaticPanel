using LuncaticPanel.Package.Server.Application.Payloads.Responses;
using LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;

namespace LuncaticPanel.Package.Server.Application.Services;

public interface IPackageDownloadResolver
{
    public Task<PackageDownloadTargetResponse> GetDownloadLocationAsync(PackageId packageId, PackageVersion packageVersion, CancellationToken ct = default);
}
