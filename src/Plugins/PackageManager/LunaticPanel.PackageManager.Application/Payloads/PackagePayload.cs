using LunaticPanel.PackageManager.Application.Payloads.Enums;

namespace LunaticPanel.PackageManager.Application.Payloads;

public record PackagePayload
{
    public PackageInfoPayload Info { get; set; } = default!;
    public string RepositorySource { get; set; } = default!;
    public RepositorySourceTypePayload RepositoryType { get; set; }
    public string Version { get; set; } = default!;
    public List<PackageDependencyPayload> Dependencies { get; set; } = new();
}
