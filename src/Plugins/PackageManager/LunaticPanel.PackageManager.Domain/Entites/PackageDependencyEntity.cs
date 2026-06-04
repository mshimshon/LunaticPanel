using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

namespace LunaticPanel.PackageManager.Domain.Entites;

public sealed record PackageDependencyEntity
{
    public PackageId Id { get; init; } = default!;

}
