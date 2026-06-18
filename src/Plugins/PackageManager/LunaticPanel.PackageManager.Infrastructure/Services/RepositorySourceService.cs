using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Requests;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using LunaticPanel.PackageManager.Application.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace LunaticPanel.PackageManager.Infrastructure.Services;

internal class RepositorySourceService : IRepositorySourceService, IExternalSourceService
{
    private const string SOURCE_FILE = @"/etc/lunaticpanel/sources.json";
    private const string SOURCE_CACHE = @"/etc/lunaticpanel/.pkg_cache";
    private const string SOURCE_CACHE_FILE_FMT = SOURCE_CACHE + @"{0}";
    private readonly ICrazyReport<RepositorySourceService> _crazyReport;
    private readonly ISafeFileWriter _safeFileWriter;
    private JsonSerializerOptions _jsonSerializer = new()
    {
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };
    public RepositorySourceService(ICrazyReport<RepositorySourceService> crazyReport, ISafeFileWriter safeFileWriter)
    {
        _crazyReport = crazyReport;
        _safeFileWriter = safeFileWriter;
        _crazyReport.SetModule("PackageManager");
    }
    public Task DownloadAsync(PackagePayload data, RepositorySourcePayload source, CancellationToken ct = default)
    {

    }





    public Task<IEnumerable<PackagePayload>> GetLatestVersionAsync(IEnumerable<string> packageIds, IReadOnlyCollection<RepositorySourcePayload> searchIn, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<IEnumerable<string>> GetVersionsAsync(string packageId, IReadOnlyCollection<RepositorySourcePayload> searchIn, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<SearchResponse<PackageInfoPayload>> SearchAsync(SearchRequest data, IReadOnlyCollection<RepositorySourcePayload> searchIn, CancellationToken ct = default) => throw new NotImplementedException();

}
