using LunaticPanel.PackageManager.Application.Payloads;

namespace LunaticPanel.PackageManager.Infrastructure.Services.Payloads.Mapping;

internal static class PluginManifestExtMapping
{

    public static PackageManifestPayload ToApplicationManifest(this PluginManifestExtPayload data)
        => new PackageManifestPayload()
        {
            Author = data.Author,
            Company = data.Company,
            Copyright = data.Copyright,
            Description = data.Description,
            DotnetVersion = data.DotnetVersion,
            Id = data.Id,
            PanelVersion = data.PanelVersion,
            PluginEntryFile = data.PluginEntryFile,
            Title = data.Title,
            Version = data.Version
        };

}
