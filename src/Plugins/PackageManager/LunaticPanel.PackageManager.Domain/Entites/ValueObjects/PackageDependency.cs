namespace LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

public sealed record PackageDependency
{
    public PackageName Name { get; }

    public PackageId Id { get; }
    public PackageVersion Version { get; }
    public PackageDependency(PackageName name, PackageId id, PackageVersion version)
    {
        Name = name;
        Id = id;
        Version = version;
    }

}
