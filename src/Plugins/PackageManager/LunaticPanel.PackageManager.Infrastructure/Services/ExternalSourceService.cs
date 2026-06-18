using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Infrastructure.Exceptions;
using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LunaticPanel.PackageManager.Infrastructure.Services;

internal class ExternalSourceService : IExternalSourceService
{
    private const string SOURCE_FILE = @"/etc/lunaticpanel/sources.json";
    private const string SOURCE_CACHE = @"/etc/lunaticpanel/.pkg_source_cache";
    private const string SOURCE_NUGET_CACHE = @"/etc/lunaticpanel/.pkg_nuget_cache";
    private const string SOURCE_CACHE_FILE_FMT = SOURCE_CACHE + @"{0}";
    private readonly ICrazyReport<RepositorySourceService> _crazyReport;
    private readonly ISafeFileWriter _safeFileWriter;
    private JsonSerializerOptions _jsonSerializer = new()
    {
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    public async Task FindAndDownloadToCache(string id, string version, CancellationToken ct = default)
    {
        var source = await GetPackageSourceForAsync(id, version, ct);
        if (source == default)
            Console.WriteLine(); // TODO: THROW
        await DownloadToCache(id, version, source, ct);
    }

    public async Task DownloadToCache(string id, string version, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
    {
        if (source.SourceType == Repositories.Payloads.Enums.ExternalSourceRepositoryTypePayload.Remote)
            await DownloadFromNugetAsync(id, version, source, ct);
        else
            await CopyFromLocalAsync(id, version, source, ct);

        string file = string.Format(SOURCE_CACHE_FILE_FMT, $"{id}.{version}.json");
        await _safeFileWriter.WriteThenCopyFileAsync(file, JsonSerializer.Serialize(source, _jsonSerializer), ct);

    }
    private async Task DownloadFromNugetAsync(string id, string version, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
    {
        var providers = Repository.Provider.GetCoreV3();
        var repo = new SourceRepository(new PackageSource(source.Source), providers);

        // Get the resource that can download packages
        var findResource = await repo.GetResourceAsync<FindPackageByIdResource>(ct);

        var nugetVersion = NuGetVersion.Parse(version);
        if (!Directory.Exists(SOURCE_NUGET_CACHE))
            Directory.CreateDirectory(SOURCE_NUGET_CACHE);

        string outputPath = Path.Combine(SOURCE_NUGET_CACHE, $"{id}.{version}.nupkg");

        using var cache = new SourceCacheContext();
        using var packageStream = File.Create(outputPath);

        bool success = await findResource.CopyNupkgToStreamAsync(id, nugetVersion, packageStream, cache, NullLogger.Instance, ct);

        if (!success)
            throw new Exception($"Failed to download {id} {version}");
    }
    private async Task CopyFromLocalAsync(string id, string version, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
    {
        if (!Directory.Exists(SOURCE_NUGET_CACHE))
            Directory.CreateDirectory(SOURCE_NUGET_CACHE);

        string outputPath = Path.Combine(SOURCE_NUGET_CACHE, $"{id}.{version}.nupkg");
        if (File.Exists(outputPath)) return;
        string inputPath = Path.Combine(source.Source, $"{id}.{version}.nupkg");
        File.Copy(inputPath, outputPath, true);
    }

    public ExternalSourceService(ICrazyReport<RepositorySourceService> crazyReport, ISafeFileWriter safeFileWriter)
    {
        _crazyReport = crazyReport;
        _safeFileWriter = safeFileWriter;
        _crazyReport.SetModule("PackageManager");
        if (!Directory.Exists(SOURCE_CACHE_FILE_FMT))
            Directory.CreateDirectory(SOURCE_CACHE_FILE_FMT);
    }

    public Task ClearSourceCacheForAsync(string id, string packageVersion, CancellationToken ct = default)
    {
        string file = string.Format(SOURCE_CACHE_FILE_FMT, $"{id}.{packageVersion}.json");
        if (File.Exists(file))
            File.Delete(file);
        return Task.CompletedTask;
    }

    public Task ClearSourceCacheAsync(string id, string packageVersion, CancellationToken ct = default)
    {
        string file = string.Format(SOURCE_CACHE_FILE_FMT, $"{id}.{packageVersion}.json");
        if (Directory.Exists(SOURCE_CACHE_FILE_FMT))
            Directory.Delete(SOURCE_CACHE_FILE_FMT, true);
        Directory.CreateDirectory(SOURCE_CACHE_FILE_FMT);
        return Task.CompletedTask;
    }
    public async Task<ExternalSourceRepositoryPayload?> GetPackageCacheSourceForAsync(string id, string packageVersion, CancellationToken ct = default)
    {
        string file = string.Format(SOURCE_CACHE_FILE_FMT, $"{id}.{packageVersion}.json");
        if (File.Exists(file))
        {
            string json = File.ReadAllText(file);
            ExternalSourceRepositoryPayload? result = JsonSerializer.Deserialize<ExternalSourceRepositoryPayload>(json, _jsonSerializer);
            if (result != default)
                return result;
        }
        return await GetPackageSourceForAsync(id, packageVersion, ct);
    }

    public async Task<ExternalSourceRepositoryPayload?> GetPackageSourceForAsync(string id, string packageVersion, CancellationToken ct = default)
    {
        string file = string.Format(SOURCE_CACHE_FILE_FMT, $"{id}.{packageVersion}.json");
        string sourceJson = File.ReadAllText(SOURCE_FILE);
        List<ExternalSourceRepositoryPayload>? configSources = JsonSerializer.Deserialize<List<ExternalSourceRepositoryPayload>>(sourceJson, _jsonSerializer);
        if (configSources == default)
            throw new SourceCorruptedException();
        if (configSources.Count <= 0)
            throw new SourceEmptyException();

        foreach (var item in configSources)
        {
            if (item.State != Repositories.Payloads.Enums.ExternalSourceRepositoryStatePayload.Enabled)
                continue;
            var package = await GetPackageInfoForAsync(id, packageVersion, item, ct);
            if (package == default) continue;
            await _safeFileWriter.WriteThenCopyFileAsync(file, JsonSerializer.Serialize(item, _jsonSerializer), ct);
            return item;
        }
        return default;
    }

    private PackagePayload GetPackageInformation(string file)
    {
        using var reader = new PackageArchiveReader(file);

        // read the nuspec
        var nuspec = reader.NuspecReader;

        // extract metadata
        string id = nuspec.GetId();
        var v = nuspec.GetVersion();
        string version = $"{v.Major}.{v.Minor}.{v.Patch}";
        string description = nuspec.GetDescription();
        string summary = nuspec.GetSummary();
        string authors = nuspec.GetAuthors();
        string title = nuspec.GetTitle();
        string projectUrl = nuspec.GetProjectUrl();
        string iconUrl = nuspec.GetIconUrl();
        //var dependencies = nuspec.GetDependencyGroups();
        return new()
        {
            Version = version,
            Info = new()
            {
                Description = summary,
                Name = title,
                PackageId = id,
                State = Application.Payloads.Enums.PackageStatePayload.Unknown
            }
        };
    }

    public async Task<PackagePayload?> GetPackageInfoForAsync(string id, string version, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
    {
        if (source.SourceType == Repositories.Payloads.Enums.ExternalSourceRepositoryTypePayload.Remote)
            return await GetPackageInfoForFromNugetAsync(id, version, source, ct);
        else
            return await GetPackageInfoForFromLocalAsync(id, version, source, ct);
    }
    public async Task<Version[]> GetVersionsForAsync(string id, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
    {
        if (source.SourceType == Repositories.Payloads.Enums.ExternalSourceRepositoryTypePayload.Remote)
            return await GetVersionsForFromNugetAsync(id, source, ct);
        else
            return await GetVersionsForFromLocalAsync(id, source, ct);
    }

    private async Task<Version[]> GetVersionsForFromNugetAsync(string id, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
    {

        var providers = Repository.Provider.GetCoreV3();
        var repo = new SourceRepository(new PackageSource("https://api.nuget.org/v3/index.json"), providers);

        var resource = await repo.GetResourceAsync<FindPackageByIdResource>();
        var versions = await resource.GetAllVersionsAsync(id, new SourceCacheContext(), NullLogger.Instance, CancellationToken.None);
        return versions.Select(p => new Version($"{p.Major}.{p.Minor}.{p.Patch}")).ToArray();
    }
    private async Task<PackagePayload?> GetPackageInfoForFromNugetAsync(string id, string version, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
    {
        var providers = Repository.Provider.GetCoreV3();
        var repo = new SourceRepository(new PackageSource(source.Source), providers);

        var metadataResource = await repo.GetResourceAsync<PackageMetadataResource>(ct);

        NuGetVersion nugetVersion = NuGetVersion.Parse(version);

        var metadata = await metadataResource.GetMetadataAsync(
            id,
            includePrerelease: true,
            includeUnlisted: true,
            sourceCacheContext: new SourceCacheContext(),
            log: NullLogger.Instance,
            token: ct);
        var result = metadata.FirstOrDefault(m =>
        {
            var a = new Version(m.Identity.Version.Major, m.Identity.Version.Minor, m.Identity.Version.Patch);
            var b = new Version(version);
            return a == b;
        });
        if (result == default) return default;
        return new()
        {
            Version = version,
            Info = new()
            {
                Description = result.Summary,
                Name = result.Title,
                PackageId = result.Identity.Id,
                State = Application.Payloads.Enums.PackageStatePayload.Unknown
            }
        };
    }

    private string[] GetLocalFileNamesFor(string id, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
        => Directory.GetFiles(source.Source, "*.nupkg")
            .Where(f => Path.GetFileName(f).StartsWith(id + ".", StringComparison.OrdinalIgnoreCase))
            .ToArray();

    private async Task<PackagePayload?> GetPackageInfoForFromLocalAsync(string id, string version, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
        => GetLocalFileNamesFor(id, source).Select(GetPackageInformation).FirstOrDefault(p => p.Version == version);

    private async Task<Version[]> GetVersionsForFromLocalAsync(string id, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
        => GetLocalFileNamesFor(id, source).Select(GetPackageInformation).Select(p => new Version(p.Version)).ToArray();


}
