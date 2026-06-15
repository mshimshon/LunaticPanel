namespace LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

public sealed record PackageDependency
{
    public PackageName Name { get; init; }

    public PackageId Id { get; init; }
    public PackageVersion Version { get; init; }
    public PackageDependency(PackageName name, PackageId id, PackageVersion version)
    {
        Name = name;
        Id = id;
        Version = version;
    }

}
