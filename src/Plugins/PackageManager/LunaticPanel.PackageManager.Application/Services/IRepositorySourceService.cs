using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Requests;
using LunaticPanel.PackageManager.Application.Payloads.Responses;

namespace LunaticPanel.PackageManager.Application.Services;

public interface IRepositorySourceService
{
    Task DownloadAsync(PackagePayload data, RepositorySourcePayload source, CancellationToken ct = default);
    Task DownloadAsync(PackagePayload data, CancellationToken ct = default);
    Task<IEnumerable<PackagePayload>> GetLatestVersionAsync(IEnumerable<string> packageIds, CancellationToken ct = default);
    Task<IEnumerable<string>> GetVersionsAsync(string packageId, CancellationToken ct = default);
    Task<SearchResponse<PackageInfoPayload>> SearchAsync(SearchRequest data, RepositorySourcePayload source, CancellationToken ct = default);
    Task<Dictionary<RepositorySourcePayload, SearchResponse<PackageInfoPayload>>> SearchAsync(SearchRequest data, CancellationToken ct = default);
}
