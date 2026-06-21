using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads;

namespace LunaticPanel.PackageManager.Infrastructure.Services;

internal interface IExternalSourceService
{
    Task FindAndDownloadToCache(string id, string version, CancellationToken ct = default);
    Task DownloadToCache(string id, string version, ExternalSourceRepositoryPayload source, CancellationToken ct = default);
    Task ClearSourceCacheForAsync(string id, string packageVersion, CancellationToken ct = default);
    Task ClearSourceCacheAsync(string id, string packageVersion, CancellationToken ct = default);
    Task<ExternalSourceRepositoryPayload?> GetPackageCacheSourceForAsync(string id, string packageVersion, CancellationToken ct = default);
    Task<ExternalSourceRepositoryPayload?> GetPackageSourceForAsync(string id, string packageVersion, CancellationToken ct = default);
    Task<PackagePayload?> FindMostRecentPackage(string id, CancellationToken ct = default);
    Task<Version[]> FindAllVersionsForAsync(string id, CancellationToken ct = default);
}
