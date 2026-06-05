using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

namespace LunaticPanel.PackageManager.Domain.Entites;

public sealed record PackageEntity
{

    public PackageEntity(PackageInfo info, RepositorySourceInfo source, PackageVersion version, ICollection<PackageDependency> dependencies)
    {
        Info = info;
        Source = source;
        Version = version;
        Dependencies = dependencies.ToList().AsReadOnly();

    }
    public PackageInfo Info { get; }
    public RepositorySourceInfo Source { get; init; } = default!;
    public PackageVersion Version { get; }
    public IReadOnlyList<PackageDependency> Dependencies { get; }

}
