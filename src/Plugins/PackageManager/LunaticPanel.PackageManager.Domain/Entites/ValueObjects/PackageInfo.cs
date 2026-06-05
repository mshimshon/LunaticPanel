using LunaticPanel.PackageManager.Domain.Entites.Enums;

namespace LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

public sealed record PackageInfo
{

    public PackageInfo(PackageId id, PackageName name, PackageDescription description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public PackageId Id { get; }
    public PackageName Name { get; }
    public PackageDescription Description { get; }
    public PackageRating? Rating { get; init; }
    public PackageAutoUpdateScore AutoUpdateScore { get; init; } = new(100);
    public PackageState State { get; init; } = PackageState.NotInstalled;

}
