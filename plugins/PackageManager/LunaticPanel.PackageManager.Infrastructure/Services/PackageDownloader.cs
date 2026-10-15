using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;
using LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Requests;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using LunaticPanel.PackageManager.Application.Services;
namespace LunaticPanel.PackageManager.Infrastructure.Services;

internal class PackageDownloader : IPackageDownloader
{
    private readonly ICrazyReport<PackageDownloader> _crazyReport;
    private readonly IExternalSourceService _sourceService;
    private readonly string _packageCache;
    public PackageDownloader(IPluginLocation pluginLocation, ICrazyReport<PackageDownloader> crazyReport,
        ISafeFileWriter safeFileWriter, IExternalSourceService sourceService)
    {
        _crazyReport = crazyReport;
        _sourceService = sourceService;
        _crazyReport.SetModule("PackageManager");
        _packageCache = pluginLocation.GetAppDataBase(".pkg_api_cache");
    }
    public async Task<string> DownloadAsync(PackageManifestPayload data, RepositorySourcePayload source, CancellationToken ct = default)
    {
        await _sourceService.DownloadToCache(data.Id, data.Version, source, ct);
        string path = Path.Combine(_packageCache, $"{data.Id}.{data.Version}.lpkg");
        if (!File.Exists(path))
            Console.WriteLine(path); // TODO: Throw Download Failed Exception
        return path;
    }

    public async Task<string> DownloadAsync(PackageManifestPayload data, CancellationToken ct = default)
    {
        await _sourceService.FindAndDownloadToCache(data.Id, data.Version, ct);
        string path = Path.Combine(_packageCache, $"{data.Id}.{data.Version}.lpkg");
        if (!File.Exists(path))
            Console.WriteLine(path); // TODO: Throw Download Failed Exception
        return path;
    }

    public async Task<IEnumerable<PackageManifestPayload>> GetLatestVersionAsync(IEnumerable<string> packageIds, CancellationToken ct = default)
    {
        List<PackageManifestPayload> result = new();
        foreach (var item in packageIds)
        {
            var package = await _sourceService.FindMostRecentPackage(item, ct);
            if (package == default) continue;
            result.Add(package);
        }
        return result;
    }

    public async Task<IEnumerable<string>> GetVersionsAsync(string packageId, CancellationToken ct = default)
    {
        var result = await _sourceService.FindAllVersionsForAsync(packageId);
        return result.Select(p => p.ToString(3));
    }

    public async Task<SearchResponse<PackageManifestPayload>> SearchAsync(SearchRequest data, RepositorySourcePayload source, CancellationToken ct = default)
    => await _sourceService.SearchAsync(data, source, ct);

    public async Task<Dictionary<RepositorySourcePayload, SearchResponse<PackageManifestPayload>>> SearchAsync(SearchRequest data, CancellationToken ct = default)
        => await _sourceService.SearchAllSourcesAsync(data, ct);
}
