namespace LunaticPanel.Engine.Domain.Plugin.ValueObjects;

public sealed record PluginIdentity(
        string PackageId,
        Version PakageVersion,
        string DisplayName,
        string? Author = default,
        string? CompanyName = default,
        string? License = default,
        string? Copyright = default
    );
