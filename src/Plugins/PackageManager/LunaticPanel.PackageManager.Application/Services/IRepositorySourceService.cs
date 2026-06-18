using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Requests;
using LunaticPanel.PackageManager.Application.Payloads.Responses;

namespace LunaticPanel.PackageManager.Application.Services;

public interface IRepositorySourceService
{
    Task<SearchResponse<PackageInfoPayload>> SearchAsync(SearchRequest data, IReadOnlyCollection<RepositorySourcePayload> searchIn, CancellationToken ct = default);
    Task<IEnumerable<PackagePayload>> GetLatestVersionAsync(IEnumerable<string> packageIds, IReadOnlyCollection<RepositorySourcePayload> searchIn, CancellationToken ct = default);
    Task<IEnumerable<string>> GetVersionsAsync(string packageId, IReadOnlyCollection<RepositorySourcePayload> searchIn, CancellationToken ct = default);
    Task DownloadAsync(PackagePayload data, RepositorySourcePayload source, CancellationToken ct = default);

}
