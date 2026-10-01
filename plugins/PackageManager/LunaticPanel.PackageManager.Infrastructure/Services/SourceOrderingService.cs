using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;
using LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Keys;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LunaticPanel.PackageManager.Infrastructure.Services;

internal class SourceOrderingService : ISourceOrderingService
{
    private readonly ISafeFileWriter _safeFileWriter;
    private readonly ISourceFileService _sourceFileProvider;
    private readonly string _sourceFile;
    private JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    public SourceOrderingService(ISafeFileWriter safeFileWriter, IPluginLocation pluginLocation, ISourceFileService sourceFileProvider)
    {
        _safeFileWriter = safeFileWriter;
        _sourceFileProvider = sourceFileProvider;
        _sourceFile = _sourceFile = pluginLocation.GetConfigFor(LPPackageManagerKeys.MODULE_NAME, "sources.json");
        if (!File.Exists(_sourceFile))
            File.Create(_sourceFile);
    }
    public async Task MoveDownAsync(RepositorySourcePayload sourcePayload, CancellationToken ct = default)
    {
        await _safeFileWriter.WriteThenCopyFileAsync(_sourceFile, content =>
        {


            var registryTask = _sourceFileProvider.GetSourcesAsync(ct);
            var registry = registryTask.Result.ToList();
            int index = registry.IndexOf(sourcePayload);
            if (index <= -1) throw new IndexOutOfRangeException("Source no longer available.");
            int swapIndex = index + 1;
            var swap = registry[swapIndex];
            registry[swapIndex] = sourcePayload;
            registry[index] = swap;
            return JsonSerializer.Serialize(registry, _jsonSerializerOptions);

        }, ct);

    }
    public Task MoveFirstAsync(RepositorySourcePayload sourcePayload, CancellationToken ct = default) => throw new NotImplementedException();
    public Task MoveLastAsync(RepositorySourcePayload sourcePayload, CancellationToken ct = default) => throw new NotImplementedException();
    public async Task MoveUpAsync(RepositorySourcePayload sourcePayload, CancellationToken ct = default)
    {
        await _safeFileWriter.WriteThenCopyFileAsync(_sourceFile, content =>
        {


            var registryTask = _sourceFileProvider.GetSourcesAsync(ct);
            var registry = registryTask.Result.ToList();
            int index = registry.IndexOf(sourcePayload);
            if (index <= -1) throw new IndexOutOfRangeException("Source no longer available.");
            int swapIndex = index - 1;
            var swap = registry[swapIndex];
            registry[swapIndex] = sourcePayload;
            registry[index] = swap;
            return JsonSerializer.Serialize(registry, _jsonSerializerOptions);

        }, ct);

    }
}
