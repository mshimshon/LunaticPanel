namespace LunaticPanel.Engine.Domain.Plugin.ValueObjects;

public sealed record PluginIdentity(
        string PackageId,
        Version PakageVersion,
        string DisplayName,
        string? Author,
        string? CompanyName,
        string? License,
        string? Copyright
    );
