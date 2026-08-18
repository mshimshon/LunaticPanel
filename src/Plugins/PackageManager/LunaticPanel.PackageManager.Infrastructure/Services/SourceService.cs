using LunaticPanel.Core.Utils.Abstraction.Plugin.Location;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Keys;
using System.Text.Json;

namespace LunaticPanel.PackageManager.Infrastructure.Services;

internal class SourceService : ISourceService
{
    private readonly string sourceFile;

    public SourceService(IPluginLocation pluginLocation)
    {
        sourceFile = pluginLocation.GetConfigFor(LPPackageManagerKeys.MODULE_NAME, "sources.json");
    }
    public async Task<ICollection<RepositorySourcePayload>> GetSourcesAsync(CancellationToken ct = default)
    {
        string jsonStr = File.ReadAllText(sourceFile);
        List<RepositorySourcePayload>? sources = JsonSerializer.Deserialize<List<RepositorySourcePayload>>(jsonStr);
        return sources ?? Array.Empty<RepositorySourcePayload>().ToList();
    }
}
