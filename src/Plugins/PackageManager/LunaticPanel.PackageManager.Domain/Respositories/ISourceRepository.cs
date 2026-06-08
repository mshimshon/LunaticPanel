using LunaticPanel.PackageManager.Domain.Entites;
using LunaticPanel.PackageManager.Domain.QueryModels.Interfaces;

namespace LunaticPanel.PackageManager.Domain.Respositories;

public interface ISourceRepository
{
    Task<IQueryModelResult<RepositorySourceEntity>> Query(IRespositorySourceQueryModel queryModel, CancellationToken ct = default);
    Task AddAsync(RepositorySourceEntity repositorySource, CancellationToken ct = default);
    Task RemoveAsync(RepositorySourceEntity repositorySource, CancellationToken ct = default);
    Task DisableAsync(RepositorySourceEntity repositorySource, CancellationToken ct = default);
    Task EnableAsync(RepositorySourceEntity repositorySource, CancellationToken ct = default);
}
