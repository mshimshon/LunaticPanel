using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;
using LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Enums;
using LunaticPanel.PackageManager.Application.Payloads.Requests;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using LunaticPanel.PackageManager.Infrastructure.Exceptions;
using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads;
using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads.Enums;
using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads.Mapping;
using LunaticPanel.PackageManager.Infrastructure.Services.Payloads;
using LunaticPanel.PackageManager.Keys;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Compression;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LunaticPanel.PackageManager.Infrastructure.Services;

internal class ExternalSourceService : IExternalSourceService, IDisposable
{
    private readonly string _sourceFile;
    private readonly string _sourceCached;
    private readonly string _sourceApiCached;
    private readonly ICrazyReport<RepositorySourceService> _crazyReport;
    private readonly ISafeFileWriter _safeFileWriter;
    private static JsonSerializerOptions _jsonSerializerOptions = new()
    {
#if DEBUG
        WriteIndented = true,
#endif
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNameCaseInsensitive = true,
    };
    public HttpClient _client;
    private bool _disposedValue;
    private readonly IPluginSystemLocation _pluginSystemLocation;
    public ExternalSourceService(IPluginLocation pluginLocation, ICrazyReport<RepositorySourceService> crazyReport, ISafeFileWriter safeFileWriter,
        IServiceProvider serviceProvider)
    {
        _crazyReport = crazyReport;
        _safeFileWriter = safeFileWriter;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);

        _sourceFile = pluginLocation.GetConfigFor(LPPackageManagerKeys.MODULE_NAME, "sources.json");
        _sourceCached = pluginLocation.GetAppDataBase(".pkg_source_cache");
        _sourceApiCached = pluginLocation.GetAppDataBase(".pkg_api_cache");
        _client = serviceProvider.GetService<IHttpClientFactory>()?.CreateClient() ??
            new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.All
            });
        _pluginSystemLocation = pluginLocation;
    }

    public async Task FindAndDownloadToCache(string id, string version, CancellationToken ct = default)
    {
        var source = await GetPackageSourceForAsync(id, version, ct);
        if (source == default)
            throw new PackageNotFoundException(id, version);
        await DownloadToCache(id, version, source, ct);
    }
    private async Task DownloadFromRemoteAsync(string id, string version, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
    {
        var relative = "lpkg/v1/package/download";
        var apiEndpoint = source.Source.EndsWith("/") ? $"{source.Source}{relative}" : $"/{source.Source}/{relative}";
        var httpResponse = await _client.GetAsync($"{apiEndpoint}/{id}/{version}");
        if (!httpResponse.IsSuccessStatusCode)
            httpResponse.EnsureSuccessStatusCode(); // TODO: THROW Deserialize Error
        var target = await httpResponse.Content.ReadFromJsonAsync<PluginDownloadExtTargetPayload>(ct);
        if (target == default)
            throw new Exception(""); // TODO: THROW Deserialize Error

        var tempPath = Path.GetTempFileName();

        using var response = await _client.GetAsync(target.Target, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        await using (var input = await response.Content.ReadAsStreamAsync())
        await using (var output = File.Create(tempPath))
        {
            await input.CopyToAsync(output);
        }
        File.Move(tempPath, Path.Combine(_sourceCached, $"{id}.{version}.lpkg"), overwrite: true);
    }
    public async Task DownloadToCache(string id, string version, RepositorySourcePayload source, CancellationToken ct = default)
    {
        if (source.SourceType == RepositorySourceTypePayload.Remote)
            await DownloadFromRemoteAsync(id, version, source.ToInfrastructurePayload(), ct);
        else
            await CopyFromLocalAsync(id, version, source.ToInfrastructurePayload(), ct);

        string file = Path.Combine(_sourceCached, $"{id}.{version}.json");
        await _safeFileWriter.WriteThenCopyFileAsync(file, JsonSerializer.Serialize(source, _jsonSerializerOptions), ct);

    }
    public async Task<PackagePayload?> GetPackageInfoForAsync(string id, string version, RepositorySourcePayload source, CancellationToken ct = default)
    {
        if (source.SourceType == RepositorySourceTypePayload.Remote)
            return await GetRemotePackageInfoAsync(id, version, source.ToInfrastructurePayload(), ct);
        else
            return await GetPackageInfoForFromLocalAsync(id, version, source.ToInfrastructurePayload(), ct);
    }
    public async Task<Version[]> FindAllVersionsForAsync(string id, CancellationToken ct = default)
    {
        if (!File.Exists(_sourceFile))
            return Array.Empty<Version>();
        string sourceJson = File.ReadAllText(_sourceFile);
        List<ExternalSourceRepositoryPayload>? configSources = JsonSerializer.Deserialize<List<ExternalSourceRepositoryPayload>>(sourceJson, _jsonSerializerOptions);
        if (configSources == default)
            throw new SourceCorruptedException();
        if (configSources.Count <= 0)
            throw new SourceEmptyException();
        List<Version> result = new();
        foreach (var item in configSources)
        {
            if (item.State != Repositories.Payloads.Enums.ExternalSourceRepositoryStatePayload.Enabled)
                continue;

            var versions = await GetVersionsForAsync(id, item.ToApplicationPayload(), ct);
            foreach (var version in versions)
            {
                if (result.Contains(version)) continue;
                result.Add(version);
            }
        }
        return result.ToArray();
    }
    public async Task<Version[]> GetVersionsForAsync(string id, RepositorySourcePayload source, CancellationToken ct = default)
    {
        if (source.SourceType == RepositorySourceTypePayload.Remote)
            return await GetRemoteVersionsAsync(id, source.ToInfrastructurePayload(), ct);
        else
            return await GetVersionsForFromLocalAsync(id, source.ToInfrastructurePayload(), ct);
    }
    public Task ClearSourceCacheForAsync(string id, string packageVersion, CancellationToken ct = default)
    {
        string file = Path.Combine(_sourceCached, $"{id}.{packageVersion}.json");
        if (File.Exists(file))
            File.Delete(file);
        return Task.CompletedTask;
    }
    public Task ClearSourceCacheAsync(CancellationToken ct = default)
    {
        if (Directory.Exists(_sourceCached))
            Directory.Delete(_sourceCached, true);
        Directory.CreateDirectory(_sourceCached);
        return Task.CompletedTask;
    }
    public async Task<RepositorySourcePayload?> GetPackageCacheSourceForAsync(string id, string packageVersion, CancellationToken ct = default)
    {
        string file = Path.Combine(_sourceCached, $"{id}.{packageVersion}.json");
        if (File.Exists(file))
        {
            string json = File.ReadAllText(file);
            ExternalSourceRepositoryPayload? result = JsonSerializer.Deserialize<ExternalSourceRepositoryPayload>(json, _jsonSerializerOptions);
            if (result != default)
                return result.ToApplicationPayload();
        }
        return await GetPackageSourceForAsync(id, packageVersion, ct);
    }

    public async Task<RepositorySourcePayload?> GetPackageSourceForAsync(string id, string version, CancellationToken ct = default)
    {
        string file = Path.Combine(_sourceCached, $"{id}.{version}.json");
        string sourceJson = File.ReadAllText(_sourceFile);
        List<ExternalSourceRepositoryPayload>? configSources = JsonSerializer.Deserialize<List<ExternalSourceRepositoryPayload>>(sourceJson, _jsonSerializerOptions);
        if (configSources == default)
            throw new SourceCorruptedException();
        if (configSources.Count <= 0)
            throw new SourceEmptyException();

        foreach (var item in configSources)
        {
            if (item.State != ExternalSourceRepositoryStatePayload.Enabled)
                continue;
            var package = await GetPackageInfoForAsync(id, version, item.ToApplicationPayload(), ct);
            if (package == default) continue;
            await _safeFileWriter.WriteThenCopyFileAsync(file, JsonSerializer.Serialize(item, _jsonSerializerOptions), ct);
            return item.ToApplicationPayload();
        }
        return default;
    }


    private async Task<PackagePayload?> GetLocalLatestVersionFromAsync(string packageId, ExternalSourceRepositoryPayload source, CancellationToken ct)
    {
        var versions = await GetPackageInfoFromLocalAsync(packageId, source);
        if (versions == null || !versions.Any())
            return default;
        var lastVersion = versions.Max(p => new Version(p.Version))!;
        return versions.FirstOrDefault(p => new Version(p.Version) == lastVersion);
    }

    public async Task<PackagePayload?> FindMostRecentPackage(string id, CancellationToken ct = default)
    {
        string sourceJson = File.ReadAllText(_sourceFile);
        List<ExternalSourceRepositoryPayload>? configSources = JsonSerializer.Deserialize<List<ExternalSourceRepositoryPayload>>(sourceJson, _jsonSerializerOptions);
        if (configSources == default)
            throw new SourceCorruptedException();
        if (configSources.Count <= 0)
            throw new SourceEmptyException();
        PackagePayload? result = default;
        foreach (var item in configSources)
        {
            if (item.State != Repositories.Payloads.Enums.ExternalSourceRepositoryStatePayload.Enabled)
                continue;

            PackagePayload? package = default;
            if (item.SourceType == Repositories.Payloads.Enums.ExternalSourceRepositoryTypePayload.Remote)
                package = await GetRemoteLatestVersionFromAsync(id, item, ct);
            else
                package = await GetLocalLatestVersionFromAsync(id, item, ct);

            if (package == default) continue;
            if (result != default)
            {
                var currentV = new Version(result.Version);
                var nextV = new Version(package.Version);
                if (currentV > nextV)
                    continue;
            }

            result = package;
        }
        return result;
    }


    private async Task<PackagePayload?> GetRemoteLatestVersionFromAsync(string id, ExternalSourceRepositoryPayload source, CancellationToken ct)
    {

        var relative = "lpkg/v1/package/latest";
        var apiEndpoint = source.Source.EndsWith("/") ? $"{source.Source}{relative}" : $"/{source.Source}/{relative}";
        var httpResponse = await _client.GetAsync($"{apiEndpoint}/{id}");
        if (!httpResponse.IsSuccessStatusCode)
            httpResponse.EnsureSuccessStatusCode(); // TODO: THROW Deserialize Error
        var manifest = await httpResponse.Content.ReadFromJsonAsync<PluginManifestExtPayload>(ct);
        if (manifest == default)
            throw new Exception(""); // TODO: THROW Deserialize Error

        return new()
        {
            Version = manifest.Version,
            Info = new()
            {
                Description = manifest.Description,
                Name = manifest.Title,
                PackageId = manifest.Id,
                State = Application.Payloads.Enums.PackageStatePayload.Unknown
            },
            RepositorySource = source.Source,
            RepositoryType = source.SourceType.ToApplicationPayload()
        };
    }
    private async Task CopyFromLocalAsync(string id, string version, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
    {
        string outputPath = Path.Combine(_sourceApiCached, $"{id}.{version}.nupkg");
        if (!File.Exists(outputPath)) return;
        string inputPath = Path.Combine(source.Source, $"{id}.{version}.nupkg");
        File.Copy(inputPath, outputPath, true);
    }

    private PackagePayload GetPackageInformation(string file)
    {
        var manifest = ReadManifestFromArchive(file);
        return new()
        {
            Version = manifest.Version,
            Info = new()
            {
                Description = manifest.Description,
                Name = manifest.Title,
                PackageId = manifest.Id,
                State = Application.Payloads.Enums.PackageStatePayload.Unknown
            }
        };
    }
    public static PluginManifestExtPayload ReadManifestFromArchive(string input)
    {
        using var zip = ZipFile.OpenRead(input);
        var entry = zip.GetEntry("manifest.json");
        if (entry == null)
            throw new Exception("manifest.json not found in package");
        using var stream = entry.Open();
        return JsonSerializer.Deserialize<PluginManifestExtPayload>(stream, _jsonSerializerOptions)!;
    }

    private async Task<Version[]> GetRemoteVersionsAsync(string id, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
    {
        var relative = "lpkg/v1/package/info";
        var apiEndpoint = source.Source.EndsWith("/") ? $"{source.Source}{relative}" : $"/{source.Source}/{relative}";
        var httpResponse = await _client.GetAsync($"{apiEndpoint}/{id}");
        if (!httpResponse.IsSuccessStatusCode)
            httpResponse.EnsureSuccessStatusCode(); // TODO: THROW Deserialize Error
        var manifest = await httpResponse.Content.ReadFromJsonAsync<List<PluginManifestExtPayload>>(ct);
        if (manifest == default)
            throw new Exception(""); // TODO: THROW Deserialize Error
        return manifest.Select(p => new Version(p.Version)).ToArray();
    }
    private async Task<PackagePayload?> GetRemotePackageInfoAsync(string id, string version, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
    {
        var relative = "lpkg/v1/package/info";
        var apiEndpoint = source.Source.EndsWith("/") ? $"{source.Source}{relative}" : $"/{source.Source}/{relative}";
        var httpResponse = await _client.GetAsync($"{apiEndpoint}/{id}/{version}");
        if (!httpResponse.IsSuccessStatusCode)
            httpResponse.EnsureSuccessStatusCode(); // TODO: THROW Deserialize Error
        var manifest = await httpResponse.Content.ReadFromJsonAsync<PluginManifestExtPayload>(ct);
        if (manifest == default)
            throw new Exception(""); // TODO: THROW Deserialize Error
        return new()
        {
            Version = manifest.Version,
            Info = new()
            {
                Description = manifest.Description,
                Name = manifest.Title,
                PackageId = manifest.Id,
                State = Application.Payloads.Enums.PackageStatePayload.Unknown
            },
            RepositorySource = source.Source,
            RepositoryType = source.SourceType.ToApplicationPayload()
        };
    }
    private string[] GetLocalFileNamesFor(string id, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
        => Directory.GetFiles(source.Source, "*.lpkg", SearchOption.AllDirectories).Where(f => Path.GetFileName(f).StartsWith(id + ".", StringComparison.OrdinalIgnoreCase))
            .ToArray();

    private async Task<PackagePayload?> GetPackageInfoForFromLocalAsync(string id, string version, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
        => GetLocalFileNamesFor(id, source).Select(GetPackageInformation).FirstOrDefault(p => p.Version == version);
    private async Task<IEnumerable<PackagePayload>> GetPackageInfoFromLocalAsync(string id, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
    => GetLocalFileNamesFor(id, source).Select(GetPackageInformation);

    private async Task<Version[]> GetVersionsForFromLocalAsync(string id, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
        => GetLocalFileNamesFor(id, source).Select(GetPackageInformation).Select(p => new Version(p.Version)).ToArray();
    public async Task<SearchResponse<PackageInfoPayload>> SearchAsync(SearchRequest data, RepositorySourcePayload source, CancellationToken ct = default)
    {
        if (source.SourceType == RepositorySourceTypePayload.Remote)
            return await SearchRemoteSourceAsync(data, source.ToInfrastructurePayload(), ct);
        else
            return await SearchLocalSourceAsync(data, source.ToInfrastructurePayload(), ct);
    }

    private async Task<SearchResponse<PackageInfoPayload>> SearchRemoteSourceAsync(SearchRequest data, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
    {
        var relative = "lpkg/v1/package/search";
        var apiEndpoint = source.Source.EndsWith("/") ? $"{source.Source}{relative}" : $"/{source.Source}/{relative}";
        string payloadData = JsonSerializer.Serialize(data);
        var payload = new StringContent(payloadData, Encoding.UTF8, "application/json");
        _crazyReport.Report(payloadData);
        var httpResponse = await _client.PostAsync($"{apiEndpoint}", payload);
        if (!httpResponse.IsSuccessStatusCode)
            httpResponse.EnsureSuccessStatusCode(); // TODO: THROW Deserialize Error
        var result = await httpResponse.Content.ReadFromJsonAsync<SearchResponse<PackageInfoPayload>>(ct);
        if (result == default)
            throw new Exception("NO RESULT DEFAULT"); // TODO: THROW Deserialize Error
        return result;
    }

    private async Task<SearchResponse<PackageInfoPayload>> SearchLocalSourceAsync(SearchRequest data, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
    {
        if (!Directory.Exists(source.Source))
            throw new Exception($"{source.Source} FOLDER SOURCE DOESN'T EXIST."); // TODO: THROW Deserialize Error
        string[]? keywords = data.Keywords?.Split(' ') ?? [];
        string[] files = Directory.GetFiles(source.Source, "*.lpkg", SearchOption.AllDirectories);
        IEnumerable<PackagePayload> result = files.Select(GetPackageInformation);
        if (keywords?.Length > 0)
            result = result.Where(p =>
             keywords.Any(k =>
                 p.Info.PackageId.Contains(k, StringComparison.OrdinalIgnoreCase) ||
                 k.Contains(p.Info.PackageId, StringComparison.OrdinalIgnoreCase)
             )
         );
        var total = result.Count();
        result = result.Skip(data.Position)
            .Take(data.MaxResult);
        return new()
        {
            Position = data.Position + result.Count(),
            Total = total,
            Result = result.Select(p => p.Info)
        };
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _client.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async Task<Dictionary<RepositorySourcePayload, SearchResponse<PackageInfoPayload>>> SearchAllSourcesAsync(SearchRequest data, CancellationToken ct = default)
    {
        string sourceJson = File.ReadAllText(_sourceFile);
        List<ExternalSourceRepositoryPayload>? configSources = JsonSerializer.Deserialize<List<ExternalSourceRepositoryPayload>>(sourceJson, _jsonSerializerOptions);
        if (configSources == default)
            throw new SourceCorruptedException();
        if (configSources.Count <= 0)
            throw new SourceEmptyException();
        Dictionary<RepositorySourcePayload, SearchResponse<PackageInfoPayload>> result = new();
        foreach (var source in configSources)
        {
            if (source.State != ExternalSourceRepositoryStatePayload.Enabled)
                continue;

            SearchResponse<PackageInfoPayload>? searchResult = default;
            // if source fails move on without crash the whole search.
            try
            {
                searchResult = await SearchAsync(data, source.ToApplicationPayload(), ct);
            }
            catch (Exception ex)
            {
                _crazyReport.ReportErrorException(ex.Message, ex);
            }

            if (searchResult == default) continue;
            result[source.ToApplicationPayload()] = searchResult;
        }
        return result;
    }

    public async Task<string[]> GetAPIVersionsAsync(RepositorySourcePayload source, CancellationToken ct = default)
    {

        var relative = "lpkg/versions";
        var apiEndpoint = source.Source.EndsWith("/") ? $"{source.Source}{relative}" : $"/{source.Source}/{relative}";
        var httpResponse = await _client.GetAsync($"{apiEndpoint}");
        if (!httpResponse.IsSuccessStatusCode)
            httpResponse.EnsureSuccessStatusCode(); // TODO: THROW Deserialize Error
        var versions = await httpResponse.Content.ReadFromJsonAsync<string[]>(ct);
        if (versions == default)
            throw new Exception(""); // TODO: THROW Deserialize Error

        return versions;
    }
}
