using LunaticPanel.PackageManager.Domain.Entites.Enums;
using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

namespace LunaticPanel.PackageManager.Domain.Entites;

public sealed record PackageEntity
{

    public PackageEntity(PackageInfo info)
    {
        Info = info;
    }
    public PackageInfo Info { get; }

    public string RespositorySource { get; init; } = default!;
    // public Version Version {get; init;} = default!;
    public PackageState State { get; init; } = PackageState.NotInstalled;
    public IReadOnlyList<PackageDependencyEntity> Dependencies { get; }

}
