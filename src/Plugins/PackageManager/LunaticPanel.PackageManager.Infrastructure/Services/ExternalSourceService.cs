using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;
using LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Infrastructure.Exceptions;
using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads;
using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads.Mapping;
using LunaticPanel.PackageManager.Keys;
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
    private readonly string _sourceFile;
    private readonly string _sourceCached;
    private readonly string _sourceApiCached;
    private readonly ICrazyReport<RepositorySourceService> _crazyReport;
    private readonly ISafeFileWriter _safeFileWriter;
    private JsonSerializerOptions _jsonSerializer = new()
    {
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };
    public ExternalSourceService(IPluginLocation pluginLocation, ICrazyReport<RepositorySourceService> crazyReport, ISafeFileWriter safeFileWriter)
    {
        _crazyReport = crazyReport;
        _safeFileWriter = safeFileWriter;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);

        _sourceFile = pluginLocation.GetConfigFor(LPPackageManagerKeys.MODULE_NAME, "sources.json");
        _sourceCached = pluginLocation.GetAppDataBase(".pkg_source_cache");
        _sourceApiCached = pluginLocation.GetAppDataBase(".pkg_api_cache");

    }

    public async Task FindAndDownloadToCache(string id, string version, CancellationToken ct = default)
    {
        var source = await GetPackageSourceForAsync(id, version, ct);
        if (source == default)
            throw new PackageNotFoundException(id, version);
        await DownloadToCache(id, version, source, ct);
    }

    public async Task DownloadToCache(string id, string version, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
    {
        if (source.SourceType == Repositories.Payloads.Enums.ExternalSourceRepositoryTypePayload.Remote)
            await DownloadFromNugetAsync(id, version, source, ct);
        else
            await CopyFromLocalAsync(id, version, source, ct);

        string file = Path.Combine(_sourceCached, $"{id}.{version}.json");
        await _safeFileWriter.WriteThenCopyFileAsync(file, JsonSerializer.Serialize(source, _jsonSerializer), ct);

    }
    public async Task<PackagePayload?> GetPackageInfoForAsync(string id, string version, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
    {
        if (source.SourceType == Repositories.Payloads.Enums.ExternalSourceRepositoryTypePayload.Remote)
            return await GetPackageInfoForFromNugetAsync(id, version, source, ct);
        else
            return await GetPackageInfoForFromLocalAsync(id, version, source, ct);
    }
    public async Task<Version[]> FindAllVersionsForAsync(string id, CancellationToken ct = default)
    {
        if (!File.Exists(_sourceFile))
            return Array.Empty<Version>();
        string sourceJson = File.ReadAllText(_sourceFile);
        List<ExternalSourceRepositoryPayload>? configSources = JsonSerializer.Deserialize<List<ExternalSourceRepositoryPayload>>(sourceJson, _jsonSerializer);
        if (configSources == default)
            throw new SourceCorruptedException();
        if (configSources.Count <= 0)
            throw new SourceEmptyException();
        List<Version> result = new();
        foreach (var item in configSources)
        {
            if (item.State != Repositories.Payloads.Enums.ExternalSourceRepositoryStatePayload.Enabled)
                continue;

            var versions = await GetVersionsForAsync(id, item, ct);
            foreach (var version in versions)
            {
                if (result.Contains(version)) continue;
                result.Add(version);
            }
        }
        return result.ToArray();
    }


    public async Task<Version[]> GetVersionsForAsync(string id, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
    {
        if (source.SourceType == Repositories.Payloads.Enums.ExternalSourceRepositoryTypePayload.Remote)
            return await GetVersionsForFromNugetAsync(id, source, ct);
        else
            return await GetVersionsForFromLocalAsync(id, source, ct);
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
    public async Task<ExternalSourceRepositoryPayload?> GetPackageCacheSourceForAsync(string id, string packageVersion, CancellationToken ct = default)
    {
        string file = Path.Combine(_sourceCached, $"{id}.{packageVersion}.json");
        if (File.Exists(file))
        {
            string json = File.ReadAllText(file);
            ExternalSourceRepositoryPayload? result = JsonSerializer.Deserialize<ExternalSourceRepositoryPayload>(json, _jsonSerializer);
            if (result != default)
                return result;
        }
        return await GetPackageSourceForAsync(id, packageVersion, ct);
    }

    public async Task<ExternalSourceRepositoryPayload?> GetPackageSourceForAsync(string id, string version, CancellationToken ct = default)
    {
        string file = Path.Combine(_sourceCached, $"{id}.{version}.json");
        string sourceJson = File.ReadAllText(_sourceFile);
        List<ExternalSourceRepositoryPayload>? configSources = JsonSerializer.Deserialize<List<ExternalSourceRepositoryPayload>>(sourceJson, _jsonSerializer);
        if (configSources == default)
            throw new SourceCorruptedException();
        if (configSources.Count <= 0)
            throw new SourceEmptyException();

        foreach (var item in configSources)
        {
            if (item.State != Repositories.Payloads.Enums.ExternalSourceRepositoryStatePayload.Enabled)
                continue;
            var package = await GetPackageInfoForAsync(id, version, item, ct);
            if (package == default) continue;
            await _safeFileWriter.WriteThenCopyFileAsync(file, JsonSerializer.Serialize(item, _jsonSerializer), ct);
            return item;
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
        List<ExternalSourceRepositoryPayload>? configSources = JsonSerializer.Deserialize<List<ExternalSourceRepositoryPayload>>(sourceJson, _jsonSerializer);
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
                package = await GetNuGetLatestVersionFromAsync(id, item, ct);
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


    private async Task<PackagePayload?> GetNuGetLatestVersionFromAsync(string packageId, ExternalSourceRepositoryPayload source, CancellationToken ct)
    {

        var providers = Repository.Provider.GetCoreV3();
        var repo = new SourceRepository(new PackageSource(source.Source), providers);

        var resource = await repo.GetResourceAsync<FindPackageByIdResource>(ct);

        using var cache = new SourceCacheContext();

        // Get all versions
        var versions = await resource.GetAllVersionsAsync(packageId, cache, NullLogger.Instance, ct);

        if (versions == null || !versions.Any())
            return default;
        var lastVersion = versions.Max()!;
        var package = await GetPackageInfoForFromNugetAsync(packageId, $"{lastVersion.Major}.{lastVersion.Minor}.{lastVersion.Patch}", source, ct);
        // NuGetVersion implements proper SemVer sorting
        return package;
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
            },
            RepositorySource = source.Source,
            RepositoryType = source.SourceType.ToApplicationPayload()
        };
    }
    private string[] GetLocalFileNamesFor(string id, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
        => Directory.GetFiles(source.Source, "*.nupkg").Where(f => Path.GetFileName(f).StartsWith(id + ".", StringComparison.OrdinalIgnoreCase))
            .ToArray();

    private async Task<PackagePayload?> GetPackageInfoForFromLocalAsync(string id, string version, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
        => GetLocalFileNamesFor(id, source).Select(GetPackageInformation).FirstOrDefault(p => p.Version == version);
    private async Task<IEnumerable<PackagePayload>> GetPackageInfoFromLocalAsync(string id, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
    => GetLocalFileNamesFor(id, source).Select(GetPackageInformation);

    private async Task<Version[]> GetVersionsForFromLocalAsync(string id, ExternalSourceRepositoryPayload source, CancellationToken ct = default)
        => GetLocalFileNamesFor(id, source).Select(GetPackageInformation).Select(p => new Version(p.Version)).ToArray();
}
