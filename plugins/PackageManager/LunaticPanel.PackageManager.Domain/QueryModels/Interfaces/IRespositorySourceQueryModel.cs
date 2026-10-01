using LunaticPanel.PackageManager.Domain.Entities.Enums;

namespace LunaticPanel.PackageManager.Domain.QueryModels.Interfaces;

public interface IRespositorySourceQueryModel : IQueryModel
{
    IReadOnlyList<RepositorySourceType> RepositorySourceTypes { get; }
    IReadOnlyList<string> Keywords { get; }
}
