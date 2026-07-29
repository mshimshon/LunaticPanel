using LunaticPanel.PackageManager.Domain.Entities.ValueObjects;

namespace LunaticPanel.PackageManager.Domain.Entities;

public sealed record PackageEntity
{
    public PackageInfo Info { get; init; }
    public RepositorySourceInfo Source { get; init; } = default!;
    public PackageVersion Version { get; init; }
    public PackagePanelVersion PanelVersion { get; init; }
    public PackageFailure? Failure { get; init; }
    public IReadOnlyList<PackageDependency> Dependencies { get; }

    public PackageEntity(PackageInfo info, RepositorySourceInfo source,
        PackageVersion version, PackagePanelVersion panelVersion,
        ICollection<PackageDependency> dependencies)
    {
        Info = info;
        Source = source;
        Version = version;
        PanelVersion = panelVersion;
        Dependencies = dependencies.ToList().AsReadOnly();

    }

    public PackageEntity(PackageInfo info, RepositorySourceInfo source,
        PackageVersion version, PackagePanelVersion panelVersion, ICollection<PackageDependency> dependencies,
        PackageFailure failure)
        : this(info, source, version, panelVersion, dependencies)
    {
        Failure = failure;
    }



}
