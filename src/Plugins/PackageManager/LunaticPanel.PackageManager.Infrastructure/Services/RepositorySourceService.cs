using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Requests;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using LunaticPanel.PackageManager.Application.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace LunaticPanel.PackageManager.Infrastructure.Services;

internal class RepositorySourceService : IRepositorySourceService
{
    private const string SOURCE_FILE = @"/var/lib/lunaticpanel/sources.json";
    private const string SOURCE_CACHE = @"/var/lib/lunaticpanel/.pkg_cache";
    private const string SOURCE_CACHE_FILE_FMT = SOURCE_CACHE + @"{0}";
    private readonly ICrazyReport<RepositorySourceService> _crazyReport;
    private readonly ISafeFileWriter _safeFileWriter;
    private readonly IExternalSourceService _sourceService;
    private JsonSerializerOptions _jsonSerializer = new()
    {
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };
    public RepositorySourceService(ICrazyReport<RepositorySourceService> crazyReport,
        ISafeFileWriter safeFileWriter, IExternalSourceService sourceService)
    {
        _crazyReport = crazyReport;
        _safeFileWriter = safeFileWriter;
        _sourceService = sourceService;
        _crazyReport.SetModule("PackageManager");
    }
    public async Task DownloadAsync(PackagePayload data, RepositorySourcePayload source, CancellationToken ct = default)
    {
        await _sourceService.DownloadToCache(data.Info.PackageId, data.Version, source, ct);
    }

    public async Task DownloadAsync(PackagePayload data, CancellationToken ct = default)
    {
        await _sourceService.FindAndDownloadToCache(data.Info.PackageId, data.Version, ct);
    }

    public async Task<IEnumerable<PackagePayload>> GetLatestVersionAsync(IEnumerable<string> packageIds, CancellationToken ct = default)
    {
        List<PackagePayload> result = new();
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
        return result.Select(p => $"{p.Major}.{p.Minor}.{p.Build}");
    }

    public async Task<SearchResponse<PackageInfoPayload>> SearchAsync(SearchRequest data, RepositorySourcePayload source, CancellationToken ct = default)
    => await _sourceService.SearchAsync(data, source, ct);

    public async Task<Dictionary<RepositorySourcePayload, SearchResponse<PackageInfoPayload>>> SearchAsync(SearchRequest data, CancellationToken ct = default)
        => await _sourceService.SearchAllSourcesAsync(data, ct);

}
