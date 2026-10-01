using LunaticPanel.PackageManager.Domain.Entities;

namespace LunaticPanel.PackageManager.Domain.Respositories;

public interface ISourceRepository
{
    Task AddAsync(RepositorySourceEntity repositorySource, CancellationToken ct = default);
    Task RemoveAsync(RepositorySourceEntity repositorySource, CancellationToken ct = default);
    Task DisableAsync(RepositorySourceEntity repositorySource, CancellationToken ct = default);
    Task EnableAsync(RepositorySourceEntity repositorySource, CancellationToken ct = default);
    Task<IEnumerable<RepositorySourceEntity>> GetAllAsync(CancellationToken ct = default);
}
