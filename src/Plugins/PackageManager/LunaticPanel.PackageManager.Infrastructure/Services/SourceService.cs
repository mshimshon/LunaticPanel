using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Keys;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LunaticPanel.PackageManager.Infrastructure.Services;

internal class SourceService : ISourceService
{
    private readonly string sourceFile;
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    public SourceService(IPluginLocation pluginLocation)
    {
        sourceFile = pluginLocation.GetConfigFor(LPPackageManagerKeys.MODULE_NAME, "sources.json");
    }

    public async Task<ICollection<RepositorySourcePayload>> GetSourcesAsync(CancellationToken ct = default)
    {
        string jsonStr = File.ReadAllText(sourceFile);
        List<RepositorySourcePayload>? sources = JsonSerializer.Deserialize<List<RepositorySourcePayload>>(jsonStr, _serializerOptions);

        return sources ?? Array.Empty<RepositorySourcePayload>().ToList();
    }

    public async Task<ICollection<RepositorySourcePayload>> SaveSourcesAsync(List<RepositorySourcePayload> sourcePayloads, CancellationToken ct = default)
    {
        string sources = JsonSerializer.Serialize(sourcePayloads, _serializerOptions);
        await File.WriteAllTextAsync(sourceFile, sources);
        return await GetSourcesAsync(ct);
    }
}
