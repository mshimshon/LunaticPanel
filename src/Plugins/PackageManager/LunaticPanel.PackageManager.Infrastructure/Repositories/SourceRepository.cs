using LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Domain.Respositories;
using LunaticPanel.PackageManager.Infrastructure.Exceptions;
using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads;
using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads.Mapping;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LunaticPanel.PackageManager.Infrastructure.Repositories;

internal class SourceRepository : ISourceRepository
{
    private readonly ISafeFileWriter _safeFileWriter;
    private const string SOURCE_FILE = @"/etc/lunaticpanel/sources.json";
    private JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    public SourceRepository(ISafeFileWriter safeFileWriter)
    {
        _safeFileWriter = safeFileWriter;
        if (!File.Exists(SOURCE_FILE))
            File.Create(SOURCE_FILE);
    }

    private IEnumerable<ExternalSourceRepositoryPayload> LoadSources(string content)
    {
        try
        {
            var sources = JsonSerializer.Deserialize<List<ExternalSourceRepositoryPayload>>(content, _jsonSerializerOptions);
            if (sources == default) throw new SourceCorruptedException();
            return sources;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task AddAsync(RepositorySourceEntity repositorySource, CancellationToken ct = default)
    {
        await _safeFileWriter.WriteThenCopyFileAsync(SOURCE_FILE, content =>
        {
            var registry = LoadSources(content).ToList();
            var entry = registry.SingleOrDefault(p => p.Source == repositorySource.Info.Source.Value);
            if (entry == default)
                registry.Add(repositorySource.ToApplicationPayload().ToInfrastructurePayload());

            return JsonSerializer.Serialize(registry, _jsonSerializerOptions);

        }, ct);
    }

    public async Task DisableAsync(RepositorySourceEntity repositorySource, CancellationToken ct = default)
    {
        await _safeFileWriter.WriteThenCopyFileAsync(SOURCE_FILE, content =>
        {
            var registry = LoadSources(content).ToList();
            var entry = registry.SingleOrDefault(p => p.Source == repositorySource.Info.Source.Value);
            if (entry == default)
                throw new SourceNotFoundException(repositorySource.Info.Source.Value);
            if (entry.State != Payloads.Enums.ExternalSourceRepositoryStatePayload.Disabled)
            {
                var i = registry.IndexOf(entry);

                registry[i] = entry with { State = Payloads.Enums.ExternalSourceRepositoryStatePayload.Disabled };
            }

            return JsonSerializer.Serialize(registry, _jsonSerializerOptions);

        }, ct);
    }

    public async Task EnableAsync(RepositorySourceEntity repositorySource, CancellationToken ct = default)
    {
        await _safeFileWriter.WriteThenCopyFileAsync(SOURCE_FILE, content =>
        {
            var registry = LoadSources(content).ToList();
            var entry = registry.SingleOrDefault(p => p.Source == repositorySource.Info.Source.Value);
            if (entry == default)
                throw new SourceNotFoundException(repositorySource.Info.Source.Value);
            if (entry.State != Payloads.Enums.ExternalSourceRepositoryStatePayload.Enabled)
            {
                var i = registry.IndexOf(entry);

                registry[i] = entry with { State = Payloads.Enums.ExternalSourceRepositoryStatePayload.Enabled };
            }

            return JsonSerializer.Serialize(registry, _jsonSerializerOptions);

        }, ct);
    }

    public Task<IEnumerable<RepositorySourceEntity>> GetAllAsync(CancellationToken ct = default)
    {
        var content = File.ReadAllText(SOURCE_FILE);
        var registry = LoadSources(content).ToList();
        var result = registry.Select(p => p.ToApplicationPayload().ToDomainEntity());
        return Task.FromResult(result);
    }
    public async Task RemoveAsync(RepositorySourceEntity repositorySource, CancellationToken ct = default)
    {
        await _safeFileWriter.WriteThenCopyFileAsync(SOURCE_FILE, content =>
        {
            var registry = LoadSources(content).ToList();
            var entry = registry.SingleOrDefault(p => p.Source == repositorySource.Info.Source.Value);
            if (entry == default)
                throw new SourceNotFoundException(repositorySource.Info.Source.Value);
            registry.Remove(entry);

            return JsonSerializer.Serialize(registry, _jsonSerializerOptions);

        }, ct);
    }
}
