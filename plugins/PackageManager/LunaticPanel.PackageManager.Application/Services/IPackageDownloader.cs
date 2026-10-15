using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Requests;
using LunaticPanel.PackageManager.Application.Payloads.Responses;

namespace LunaticPanel.PackageManager.Application.Services;

public interface IPackageDownloader
{
    Task<string> DownloadAsync(PackageManifestPayload data, CancellationToken ct = default);
    Task<IEnumerable<PackageManifestPayload>> GetLatestVersionAsync(IEnumerable<string> packageIds, CancellationToken ct = default);
    Task<IEnumerable<string>> GetVersionsAsync(string packageId, CancellationToken ct = default);
    Task<SearchResponse<PackageManifestPayload>> SearchAsync(SearchRequest data, RepositorySourcePayload source, CancellationToken ct = default);
    Task<Dictionary<RepositorySourcePayload, SearchResponse<PackageManifestPayload>>> SearchAsync(SearchRequest data, CancellationToken ct = default);
}
