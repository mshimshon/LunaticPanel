using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;
using LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;
using LunaticPanel.PackageManager.Application.Payloads.Enums;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Domain.Respositories;
using LunaticPanel.PackageManager.Infrastructure.Exceptions;
using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads.Mapping;
using LunaticPanel.PackageManager.Keys;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LunaticPanel.PackageManager.Infrastructure.Repositories;

internal class SourceRepository : ISourceRepository
{
    private readonly ISafeFileWriter _safeFileWriter;
    private readonly ISourceFileService _sourceFileProvider;
    private readonly string _sourceFile;
    private JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    public SourceRepository(ISafeFileWriter safeFileWriter, IPluginLocation pluginLocation, ISourceFileService sourceFileProvider)
    {
        _safeFileWriter = safeFileWriter;
        _sourceFileProvider = sourceFileProvider;
        _sourceFile = _sourceFile = pluginLocation.GetConfigFor(LPPackageManagerKeys.MODULE_NAME, "sources.json");
        if (!File.Exists(_sourceFile))
            File.Create(_sourceFile);
    }

    public async Task AddAsync(RepositorySourceEntity repositorySource, CancellationToken ct = default)
    {
        await _safeFileWriter.WriteThenCopyFileAsync(_sourceFile, content =>
        {


            var registryTask = _sourceFileProvider.GetSourcesAsync(ct);
            var registry = registryTask.Result.ToList();
            var entry = registry.SingleOrDefault(p => p.Source == repositorySource.Info.Source.Value);
            if (entry == default)
                registry.Add(repositorySource.ToApplicationPayload());
            return JsonSerializer.Serialize(registry, _jsonSerializerOptions);

        }, ct);
    }

    public async Task DisableAsync(RepositorySourceEntity repositorySource, CancellationToken ct = default)
    {
        await _safeFileWriter.WriteThenCopyFileAsync(_sourceFile, content =>
        {
            var registryTask = _sourceFileProvider.GetSourcesAsync(ct);
            List<Application.Payloads.RepositorySourcePayload>? registry = registryTask.Result.ToList();
            var entry = registry.SingleOrDefault(p => p.Source == repositorySource.Info.Source.Value);
            if (entry == default)
                throw new SourceNotFoundException(repositorySource.Info.Source.Value);
            if (entry.State != RepositorySourceStatePayload.Disabled)
            {
                var i = registry.IndexOf(entry);

                registry[i] = entry with { State = RepositorySourceStatePayload.Disabled };
            }

            return JsonSerializer.Serialize(registry, _jsonSerializerOptions);

        }, ct);
    }

    public async Task EnableAsync(RepositorySourceEntity repositorySource, CancellationToken ct = default)
    {
        await _safeFileWriter.WriteThenCopyFileAsync(_sourceFile, content =>
        {
            var registryTask = _sourceFileProvider.GetSourcesAsync(ct);
            List<Application.Payloads.RepositorySourcePayload>? registry = registryTask.Result.ToList();
            var entry = registry.SingleOrDefault(p => p.Source == repositorySource.Info.Source.Value);
            if (entry == default)
                throw new SourceNotFoundException(repositorySource.Info.Source.Value);
            if (entry.State != RepositorySourceStatePayload.Enabled)
            {
                var i = registry.IndexOf(entry);
                registry[i] = entry with { State = RepositorySourceStatePayload.Enabled };
            }

            return JsonSerializer.Serialize(registry, _jsonSerializerOptions);

        }, ct);
    }

    public async Task<IEnumerable<RepositorySourceEntity>> GetAllAsync(CancellationToken ct = default)
    {
        var content = File.ReadAllText(_sourceFile);
        var registry = await _sourceFileProvider.GetSourcesAsync(ct);
        var result = registry.Select(p => p.ToDomainEntity());
        return result;
    }
    public async Task RemoveAsync(RepositorySourceEntity repositorySource, CancellationToken ct = default)
    {
        await _safeFileWriter.WriteThenCopyFileAsync(_sourceFile, content =>
        {
            var registryTask = _sourceFileProvider.GetSourcesAsync(ct);
            List<Application.Payloads.RepositorySourcePayload>? registry = registryTask.Result.ToList();
            var entry = registry.SingleOrDefault(p => p.Source == repositorySource.Info.Source.Value);
            if (entry == default)
                throw new SourceNotFoundException(repositorySource.Info.Source.Value);
            registry.Remove(entry);

            return JsonSerializer.Serialize(registry, _jsonSerializerOptions);

        }, ct);
    }
}
