using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Requests;
using LunaticPanel.PackageManager.Application.Payloads.Responses;

namespace LunaticPanel.PackageManager.Infrastructure.Services;

internal interface IExternalSourceService
{
    Task FindAndDownloadToCache(string id, string version, CancellationToken ct = default);
    Task DownloadToCache(string id, string version, RepositorySourcePayload source, CancellationToken ct = default);
    Task ClearSourceCacheForAsync(string id, string packageVersion, CancellationToken ct = default);
    Task ClearSourceCacheAsync(CancellationToken ct = default);
    Task<RepositorySourcePayload?> GetPackageCacheSourceForAsync(string id, string packageVersion, CancellationToken ct = default);
    Task<RepositorySourcePayload?> GetPackageSourceForAsync(string id, string packageVersion, CancellationToken ct = default);
    Task<PackagePayload?> FindMostRecentPackage(string id, CancellationToken ct = default);
    Task<Version[]> FindAllVersionsForAsync(string id, CancellationToken ct = default);
    Task<Dictionary<RepositorySourcePayload, SearchResponse<PackageInfoPayload>>> SearchAllSourcesAsync(SearchRequest data, CancellationToken ct = default);
    Task<SearchResponse<PackageInfoPayload>> SearchAsync(SearchRequest data, RepositorySourcePayload source, CancellationToken ct = default);

}
