using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Requests;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using LunaticPanel.PackageManager.Application.Services;

namespace LunaticPanel.PackageManager.Tests.Application.Mock;

internal class RepositorySourceService : IPackageDownloader
{
    List<PackageManifestPayload> _cache = new()
    {

        new PackageManifestPayload(){
            Title = "Package Test",
            Description = "No Descriptions",
            Id = "Package.Test",
            Version = "0.1.1",
            Author = "Maksim Shimshon", Company = "Maksim Shimshon",
            Copyright = "Maksim Shimshon (c) 2023", PanelVersion = "0.1.22", PluginEntryFile="xxx.dll", DotnetVersion="0"
        },
        new PackageManifestPayload(){
            Title = "Package Test 2",
            Description = "No Descriptions",
            Id = "Package.Test",
            Version = "0.1.1",
            Author = "Maksim Shimshon", Company = "Maksim Shimshon",
            Copyright = "Maksim Shimshon (c) 2023", PanelVersion = "0.1.22", PluginEntryFile="xxx.dll", DotnetVersion="0"
        }
    };
    public static List<string> Downloaded { get; set; } = new();
    public async Task<string> DownloadAsync(PackageManifestPayload data, RepositorySourcePayload source, CancellationToken ct = default)
    {
        string filename = $"{data.Id}.{data.Version}.nupkg";
        if (!Downloaded.Contains(filename))
            Downloaded.Add(filename);
        return await DownloadAsync(data, ct);
    }

    public Task<string> DownloadAsync(PackageManifestPayload data, CancellationToken ct = default)
    {
        string filename = $"{data.Id}.{data.Version}.nupkg";
        if (!Downloaded.Contains(filename))
            Downloaded.Add(filename);
        return Task.FromResult(filename);
    }



    public Task<IEnumerable<PackageManifestPayload>> GetLatestVersionAsync(IEnumerable<string> packageIds, IReadOnlyCollection<RepositorySourcePayload> searchIn, CancellationToken ct = default)
    {
        IEnumerable<PackageManifestPayload> result = _cache;
        return Task.FromResult(result);
    }
    public Task<IEnumerable<PackageManifestPayload>> GetLatestVersionAsync(IEnumerable<string> packageIds, CancellationToken ct = default) => throw new NotImplementedException();

    public Task<IEnumerable<string>> GetVersionsAsync(string packageId, IReadOnlyCollection<RepositorySourcePayload> searchIn, CancellationToken ct = default)
    {
        IEnumerable<string> result = _cache.Where(p => p.Id == packageId).Select(p => p.Version);
        return Task.FromResult(result);
    }

    public Task<SearchResponse<PackageManifestPayload>> SearchAsync(SearchRequest data, IReadOnlyCollection<RepositorySourcePayload> searchIn, CancellationToken ct = default)
    {
        ICollection<PackageManifestPayload> list = _cache.ToList();
        SearchResponse<PackageManifestPayload> result = new()
        {
            Result = list,
            Position = data.Position,
            TotalResults = list.Count
        };
        return Task.FromResult(result);
    }

    public Task<IEnumerable<string>> GetVersionsAsync(string packageId, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<SearchResponse<PackageManifestPayload>> SearchAsync(SearchRequest data, RepositorySourcePayload source, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<Dictionary<RepositorySourcePayload, SearchResponse<PackageManifestPayload>>> SearchAsync(SearchRequest data, CancellationToken ct = default) => throw new NotImplementedException();
}
