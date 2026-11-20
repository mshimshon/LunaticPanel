using LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads.Enums;
using LunaticPanel.PackageManager.Infrastructure.Services.Payloads;
using NuGet.Versioning;
using System.Text.Json.Serialization;

namespace LunaticPanel.PackageManager.Infrastructure.Repositories.Payloads;

internal sealed record ExternalPluginPayload
{
    public string Id => Manifest.Id;
    public PluginManifestExtPayload Manifest { get; init; } = default!;
    public ExternalPluginStartupState StartupState { get; init; }
    public string? Failure { get; init; }

}
