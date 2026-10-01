using LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework.Models.Contracts;

namespace LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework.Models;

public sealed record PackageInfoModel : IModelTimestamps
{
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public string Version { get; set; } = default!;
    public string PanelVersion { get; set; } = default!;
    public string DotnetVersion { get; set; } = default!;
    public string PluginEntryFile { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Author { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Copyright { get; set; }
    public bool Hidden { get; set; }
    public PackageModel Package { get; set; } = default!;
    public string PackageId { get; set; } = default!;
}
