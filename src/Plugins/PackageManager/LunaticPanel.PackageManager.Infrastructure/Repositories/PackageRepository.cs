using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;
using LunaticPanel.PackageManager.Domain.Entites;
using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;
using LunaticPanel.PackageManager.Domain.QueryModels.Interfaces;
using LunaticPanel.PackageManager.Domain.Respositories;
using LunaticPanel.PackageManager.Infrastructure.Exceptions;
using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads;
using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads.Mapping;
using LunaticPanel.PackageManager.Infrastructure.Services.Payloads;
using LunaticPanel.PackageManager.Keys;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LunaticPanel.PackageManager.Infrastructure.Repositories;

internal class PackageRepository : IPackageRepository
{
    private const string BOOTSTRAP_LOCATION = "/var/lib/lunaticpanel/config/bootstrap.json";
    private const string PLUGIN_LOCATION = "/srv/lunaticpanel/plugins/";
    private const string BOOTSTRAP_PLUGIN_LOCATION_FMT = PLUGIN_LOCATION + "{0}";
    private readonly ISafeFileWriter _safeFileWriter;
    private readonly ICrazyReport<PackageRepository> _crazyReport;
    private JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    public PackageRepository(ISafeFileWriter safeFileWriter, ICrazyReport<PackageRepository> crazyReport)
    {
        _safeFileWriter = safeFileWriter;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report($"Checking if '{BOOTSTRAP_LOCATION}' exist");
        if (!File.Exists(BOOTSTRAP_LOCATION))
            throw new BootstrapNotFoundException();
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
            if (entry.Entity.Lifecycle.State == Payloads.Enums.ExternalPluginEntityLifecycleState.Active)
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
                         StartupState = Payloads.Enums.ExternalPluginEntityLifecycleStartupState.Disabled
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
                        StartupState = Payloads.Enums.ExternalPluginEntityLifecycleStartupState.Disabled
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
        var mockRepos = new RepositorySourceInfo(new RepositorySourceLocal("local"), Domain.Entites.Enums.RepositorySourceType.Local);
        ICollection<PackageEntity> result = validPlugins?
            .Where(p => p != null)
            .Select(p =>
            {
                return p!.Entity.MapToDomainEntity(mockRepos);
            })
            .ToList() ?? Array.Empty<PackageEntity>().ToList();
        return Task.FromResult(result);
    }
    public Task<PackageEntity> GetByIdAsync(PackageId id, CancellationToken ct = default) => throw new NotImplementedException();
    public Task InstallAsync(PackageEntity package, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<IQueryModelResult<PackageInfo>> QueryAsync(IPackageQueryModel queryModel, CancellationToken ct = default) => throw new NotImplementedException();
    public Task UpdateAsync(PackageEntity target, CancellationToken ct = default) => throw new NotImplementedException();
}
