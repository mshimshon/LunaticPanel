using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;
using LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Domain.Entities.ValueObjects;
using LunaticPanel.PackageManager.Domain.QueryModels.Interfaces;
using LunaticPanel.PackageManager.Infrastructure.Exceptions;
using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads;
using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads.Enums;
using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads.Mapping;
using LunaticPanel.PackageManager.Infrastructure.Services.Payloads;
using LunaticPanel.PackageManager.Keys;
using System.Text.Json;
using System.Text.Json.Serialization;
using static LunaticPanel.PackageManager.Infrastructure.Extensions.PackageFileExt;
namespace LunaticPanel.PackageManager.Infrastructure.Services;

internal class HostDiskPackageService : IHostDiskPackageService
{
    private readonly ICrazyReport<HostDiskPackageService> _crazyReport;
    private readonly string _applyLocation;
    private readonly string _installedLocation;
    private readonly string _rollbackLocation;
    private readonly string _deleteLocation;
    private readonly string _preInstalledFolder;

    private const string BOOTSTRAP_LOCATION = "/var/lib/lunaticpanel/config/bootstrap.json";
    private readonly string _sourceCached;
    private readonly string _sourceApiCached;

    private const string PLUGIN_LOCATION = "/srv/lunaticpanel/plugins/";
    private const string BOOTSTRAP_PLUGIN_LOCATION_FMT = PLUGIN_LOCATION + "{0}";
    private readonly ISafeFileWriter _safeFileWriter;
    private JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };
    public HostDiskPackageService(IPluginLocation pluginLocation, ISafeFileWriter safeFileWriter, ICrazyReport<HostDiskPackageService> crazyReport)
    {
        _safeFileWriter = safeFileWriter;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report($"Checking if '{BOOTSTRAP_LOCATION}' exist");
        if (!File.Exists(BOOTSTRAP_LOCATION))
            throw new BootstrapNotFoundException();

        var tempRoot = Path.Combine(Path.GetTempPath(), "lunaticpanel", ".plugins");
        _applyLocation = Path.Combine(tempRoot, "apply");
        _installedLocation = Path.Combine(tempRoot, "installed");
        _rollbackLocation = Path.Combine(tempRoot, "rollbacks");
        _deleteLocation = Path.Combine(tempRoot, "delete");
        _preInstalledFolder = Path.Combine(Environment.CurrentDirectory, "plugins_preinstalled");

        _sourceCached = pluginLocation.GetAppDataBase(".pkg_source_cache");
        _sourceApiCached = pluginLocation.GetAppDataBase(".pkg_api_cache");
    }

    public Task<ICollection<PackagePayload>> GetIntalled(CancellationToken ct = default)
    {
        ICollection<PackagePayload> result = FolderToPackagePayload(_installedLocation);
        return Task.FromResult(result);
    }
    public Task<ICollection<PackagePayload>> GetPendingDelete(CancellationToken ct = default)
    {
        ICollection<PackagePayload> result = FolderToPackagePayload(_deleteLocation);
        return Task.FromResult(result);
    }
    public Task<ICollection<PackagePayload>> GetPendingUpdates(CancellationToken ct = default)
    {
        ICollection<PackagePayload> result = FolderToPackagePayload(_applyLocation);
        return Task.FromResult(result);
    }
    public Task<ICollection<PackagePayload>> GetRollbacks(CancellationToken ct = default)
    {
        ICollection<PackagePayload> result = FolderToPackagePayload(_rollbackLocation);
        return Task.FromResult(result);
    }

    public Task<ICollection<PackagePayload>> GetPreInstalled(CancellationToken ct = default)
    {
        ICollection<PackagePayload> result = FolderToPackagePayload(_preInstalledFolder);
        return Task.FromResult(result);
    }

    private List<PackagePayload> FolderToPackagePayload(string path)
    {

        if (!Directory.Exists(path)) return new();
        string[] packages = Directory.GetFiles(path, "*.lpkg", SearchOption.TopDirectoryOnly);
        var result = new List<PackagePayload>();
        foreach (var item in packages)
        {
            var manifest = GetPackageInformation(item);
            if (result.Any(p => p.Info.PackageId == manifest.Info.PackageId))
                continue;
            result.Add(manifest);

        }
        return result;
    }


    private ExternalBootstrapPayload Loadbootstrap(string content)
    {
        _crazyReport.Report("Reading Bootstrap File");

        try
        {
            var bootstrap = JsonSerializer.Deserialize<ExternalBootstrapPayload>(content, _jsonSerializerOptions);
            if (bootstrap == default) throw new BootstrapCorruptedException();
            return bootstrap;
        }
        catch (Exception ex)
        {
            _crazyReport.ReportErrorException(ex.Message, ex);

            throw;
        }
    }

    public async Task DeleteAsync(PackageId id, CancellationToken ct = default)
    {
        await _safeFileWriter.WriteThenCopyFileAsync(BOOTSTRAP_LOCATION, (content) =>
        {
            string pluginFolder = string.Format(BOOTSTRAP_PLUGIN_LOCATION_FMT, id.Value.Replace('.', '_').ToLower());
            var bootstrap = Loadbootstrap(content);
            var entry = bootstrap.KnownPlugins.SingleOrDefault(p => p.Entity.Identity.PackageId == id.Value);
            if (entry == default) throw new BootstrapPackageNotFoundException(id.Value);
            if (entry.Entity.Lifecycle.State == ExternalPluginEntityLifecycleState.Active)
                throw new BootstrapPackageDeleteActiveException(id.Value);
            bootstrap.KnownPlugins.Remove(entry);
            Directory.Delete(pluginFolder);
            return JsonSerializer.Serialize(bootstrap, _jsonSerializerOptions);
        }, ct);
    }
    public async Task DisableAsync(PackageId id, CancellationToken ct = default)
    {
        await _safeFileWriter.WriteThenCopyFileAsync(BOOTSTRAP_LOCATION, (content) =>
        {
            var bootstrap = Loadbootstrap(content);
            var entry = bootstrap.KnownPlugins.SingleOrDefault(p => p.Entity.Identity.PackageId == id.Value);
            if (entry == default) throw new BootstrapPackageNotFoundException(id.Value);
            bootstrap.KnownPlugins.Remove(entry);
            entry = entry with
            {
                Entity = entry.Entity with
                {
                    Lifecycle = entry.Entity.Lifecycle with
                    {
                        StartupState = ExternalPluginEntityLifecycleStartupState.Disabled
                    }
                }
            };
            bootstrap.KnownPlugins.Add(entry);
            return JsonSerializer.Serialize(bootstrap, _jsonSerializerOptions);
        }, ct);
    }
    public async Task EnableAsync(PackageId id, CancellationToken ct = default)
    {
        await _safeFileWriter.WriteThenCopyFileAsync(BOOTSTRAP_LOCATION, (content) =>
        {
            var bootstrap = Loadbootstrap(content);
            var entry = bootstrap.KnownPlugins.SingleOrDefault(p => p.Entity.Identity.PackageId == id.Value);
            if (entry == default) throw new BootstrapPackageNotFoundException(id.Value);
            bootstrap.KnownPlugins.Remove(entry);
            entry = entry with
            {
                Entity = entry.Entity with
                {
                    Lifecycle = entry.Entity.Lifecycle with
                    {
                        StartupState = ExternalPluginEntityLifecycleStartupState.Disabled
                    }
                }
            };
            bootstrap.KnownPlugins.Add(entry);
            return JsonSerializer.Serialize(bootstrap, _jsonSerializerOptions);
        }, ct);
    }

    public Task<ICollection<PackageEntity>> GetAll(CancellationToken ct = default)
    {
        var bootstrap = Loadbootstrap(File.ReadAllText(BOOTSTRAP_LOCATION));
        var validPlugins = bootstrap.KnownPlugins
            .Select(p =>
            {
                string manifestFile = Path.Combine(p.PluginDir, "manifest.json");
                if (!File.Exists(manifestFile)) return null;
                PluginManifestExtPayload? manifest = JsonSerializer.Deserialize<PluginManifestExtPayload>(File.ReadAllText(manifestFile));
                if (manifest == default) return null;
                var result = p with
                {
                    Entity = p.Entity with
                    {
                        Identity = p.Entity.Identity with
                        {
                            Description = manifest.Description,
                            Author = manifest.Author,
                            CompanyName = manifest.Company,
                            Copyright = manifest.Copyright,
                            DisplayName = manifest.Title,
                            PakageVersion = manifest.Version,
                            PackageId = manifest.Id,
                            PanelVersion = manifest.PanelVersion,
                        }
                    }
                };
                return result;
            })
            .ToList();
        // Create Local/Fake Source for PAckage as we don't save the source along the plugin.
        var mockRepos = new RepositorySourceInfo(new RepositorySourceLocal("local"), Domain.Entities.Enums.RepositorySourceType.Local);
        ICollection<PackageEntity> result = validPlugins?
            .Where(p => p != null)
            .Select(p =>
            {
                return p!.Entity.MapToDomainEntity(mockRepos);
            })
            .ToList() ?? Array.Empty<PackageEntity>().ToList();
        return Task.FromResult(result);
    }
    public async Task<PackageEntity> GetByIdAsync(PackageId id, CancellationToken ct = default)
    {
        var all = await GetAll(ct);
        return all.Single(p => p.Info.Id == id);
    }
    public Task InstallAsync(PackageEntity package, CancellationToken ct = default)
    {
        // We Have Downloaded Package REady to Install from Cache
        string filename = $"{package.Info.Id.Value}.{package.Version.Value}.lpkg";
        string file = Path.Combine(_sourceApiCached, filename);
        if (!File.Exists(file))
            throw new InstallNotFoundException(package.Info.Id.Value);
        string output = Path.Combine(_applyLocation, filename);
        File.Move(file, output);
        return Task.CompletedTask;
    }
    public Task<IQueryModelResult<PackageInfo>> QueryAsync(IPackageQueryModel queryModel, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task UpdateAsync(PackageEntity target, CancellationToken ct = default)
        => InstallAsync(target, ct);
}
