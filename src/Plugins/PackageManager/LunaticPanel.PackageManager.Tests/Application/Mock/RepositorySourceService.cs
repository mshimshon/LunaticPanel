using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Requests;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using LunaticPanel.PackageManager.Application.Services;

namespace LunaticPanel.PackageManager.Tests.Application.Mock;

internal class RepositorySourceService : IRepositorySourceService
{
    public static List<string> Downloaded { get; set; } = new();
    public Task DownloadAsync(PackagePayload data, RepositorySourcePayload source, CancellationToken ct = default)
    {
        string filename = $"{data.Info.PackageId}.v{data.Version}.nupkg";
        if (!Downloaded.Contains(filename))
            Downloaded.Add(filename);
        return Task.CompletedTask;
    }

    List<PackagePayload> _cache = new()
    {

        new PackagePayload(){ Info = new()
        {
            Name = "Package Test",
            Description = "No Descriptions",
            PackageId = "Package.Test",
            State = PackageManager.Application.Payloads.Enums.PackageStatePayload.Unknown,
        },
            RepositorySource = "local://myfolder/", RepositoryType = PackageManager.Application.Payloads.Enums.RepositorySourceTypePayload.Local,
            Version = "0.1.1"
        },
        new PackagePayload(){ Info = new()
        {
            Name = "Package Test 2",
            Description = "No Descriptions",
            PackageId = "Package.Test.Two",
            State = PackageManager.Application.Payloads.Enums.PackageStatePayload.Unknown,
        },
            RepositorySource = "local://myfolder/", RepositoryType = PackageManager.Application.Payloads.Enums.RepositorySourceTypePayload.Local,
            Version = "0.2.1"
        }
    };

    public Task<IEnumerable<PackagePayload>> GetLatestVersionAsync(IEnumerable<string> packageIds, IReadOnlyCollection<RepositorySourcePayload> searchIn, CancellationToken ct = default)
    {
        IEnumerable<PackagePayload> result = _cache;
        return Task.FromResult(result);
    }

    public Task<IEnumerable<string>> GetVersionsAsync(string packageId, IReadOnlyCollection<RepositorySourcePayload> searchIn, CancellationToken ct = default)
    {
        IEnumerable<string> result = _cache.Where(p => p.Info.PackageId == packageId).Select(p => p.Version);
        return Task.FromResult(result);
    }

    public Task<SearchResponse<PackageInfoPayload>> SearchAsync(SearchRequest data, IReadOnlyCollection<RepositorySourcePayload> searchIn, CancellationToken ct = default)
    {
        ICollection<PackageInfoPayload> list = _cache.Select(p => p.Info).ToList();
        SearchResponse<PackageInfoPayload> result = new()
        {
            Result = list,
            Position = data.Position,
            Total = list.Count
        };
        return Task.FromResult(result);
    }
}
