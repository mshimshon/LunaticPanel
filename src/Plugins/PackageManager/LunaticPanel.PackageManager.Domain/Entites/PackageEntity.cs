using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

namespace LunaticPanel.PackageManager.Domain.Entites;

public sealed record PackageEntity
{
    public PackageInfo Info { get; init; }
    public RepositorySourceInfo Source { get; init; } = default!;
    public PackageVersion Version { get; init; }
    public PackageFailure? Failure { get; init; }
    public IReadOnlyList<PackageDependency> Dependencies { get; }

    public PackageEntity(PackageInfo info, RepositorySourceInfo source,
        PackageVersion version,
        ICollection<PackageDependency> dependencies)
    {
        Info = info;
        Source = source;
        Version = version;
        Dependencies = dependencies.ToList().AsReadOnly();

    }

    public PackageEntity(PackageInfo info, RepositorySourceInfo source,
        PackageVersion version, ICollection<PackageDependency> dependencies,
        PackageFailure failure)
        : this(info, source, version, dependencies)
    {
        Failure = failure;
    }



}
