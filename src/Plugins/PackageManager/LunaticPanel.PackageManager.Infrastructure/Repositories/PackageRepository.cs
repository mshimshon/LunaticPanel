using LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;
using LunaticPanel.PackageManager.Domain.Entites;
using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;
using LunaticPanel.PackageManager.Domain.QueryModels.Interfaces;
using LunaticPanel.PackageManager.Domain.Respositories;
using LunaticPanel.PackageManager.Infrastructure.Exceptions;
using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads;
using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads.Mapping;
using LunaticPanel.PackageManager.Infrastructure.Services.Payloads;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LunaticPanel.PackageManager.Infrastructure.Repositories;

internal class PackageRepository : IPackageRepository
{
    private const string BOOTSTRAP_LOCATION = "/var/lib/lunaticpanel/config/bootstrap.json";
    private const string BOOTSTRAP_PLUGIN_LOCATION_FMT = "/srv/lunaticpanel/plugins/{0}";
    private readonly ISafeFileWriter _safeFileWriter;
    private ExternalBootstrapPayload _bootstrap = default!;
    private JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    public PackageRepository(ISafeFileWriter safeFileWriter)
    {
        _safeFileWriter = safeFileWriter;
        if (!File.Exists(BOOTSTRAP_LOCATION))
            throw new BootstrapNotFoundException();
    }

    private ExternalBootstrapPayload Loadbootstrap(string content)
    {
        try
        {
            var bootstrap = JsonSerializer.Deserialize<ExternalBootstrapPayload>(content, _jsonSerializerOptions);
            if (bootstrap == default) throw new BootstrapCorruptedException();
            return bootstrap;
        }
        catch (Exception)
        {
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
        var validPlugins = _bootstrap.KnownPlugins
            .Select(p =>
            {
                string manifestFile = Path.Combine(p.PluginDir, "manifest.json");
                if (!File.Exists(manifestFile)) return null;
                PluginManifestExtPayload? manifest = JsonSerializer.Deserialize<PluginManifestExtPayload>(File.ReadAllText(manifestFile));
                if (manifest == default) return null;
                return p with
                {
                    Entity = p.Entity with
                    {
                        Identity = p.Entity.Identity with
                        {
                            Description = manifest.Description
                        }
                    }
                };
            })
            .Where(p => p != default);
        ICollection<PackageEntity> result = validPlugins
            .Select(p => p!.Entity.MapToDomainEntity(new(new RepositorySourceLocal("local"), Domain.Entites.Enums.RepositorySourceType.Local)))
            .ToList();
        return Task.FromResult(result);
    }
    public Task<PackageEntity> GetByIdAsync(PackageId id, CancellationToken ct = default) => throw new NotImplementedException();
    public Task InstallAsync(PackageEntity package, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<IQueryModelResult<PackageInfo>> QueryAsync(IPackageQueryModel queryModel, CancellationToken ct = default) => throw new NotImplementedException();
    public Task UpdateAsync(PackageEntity target, CancellationToken ct = default) => throw new NotImplementedException();
}
