using LunaticPanel.PackageManager.Domain.Entites.Enums;
using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;
using LunaticPanel.PackageManager.Domain.QueryModels.Enums;

namespace LunaticPanel.PackageManager.Domain.QueryModels.Interfaces;

public interface IPackageQueryModel : IQueryModel
{
    IReadOnlyList<RepositorySourceInfo> RepositorySources { get; }
    IReadOnlyList<PackageState> PackageStates { get; }
    IReadOnlyList<PackageId> PackageIds { get; }
    IReadOnlyList<string> Keywords { get; }
    PackageVersion? SpecificVersion { get; }
    ValueComparision SpecificVersionComparision { get; }
}
