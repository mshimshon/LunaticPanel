namespace LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework.Models;

public sealed record PackageInfoModel
{
    public DateTime Created { get; init; }
    public DateTime Updated { get; init; }
    public string Version { get; init; } = default!;
    public string PanelVersion { get; init; } = default!;
    public string DotnetVersion { get; init; } = default!;
    public string PluginEntryFile { get; init; } = default!;
    public string Description { get; init; } = default!;
    public string Author { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string? Copyright { get; init; }
    public bool Hidden { get; init; }
    public PackageModel Package { get; init; } = default!;
    public string PackageId { get; init; } = default!;
}
