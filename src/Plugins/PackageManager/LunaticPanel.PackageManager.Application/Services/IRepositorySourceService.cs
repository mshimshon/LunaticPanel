using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Responses;

namespace LunaticPanel.PackageManager.Application.Services;

public interface IRepositorySourceService
{
    Task<SearchResponse<PackageInfoPayload>> SearchAsync(string q, IReadOnlyCollection<RepositorySourcePayload> searchIn, CancellationToken ct = default);
    Task DownloadAsync(string id, string version, RepositorySourcePayload source, CancellationToken ct = default);
}
