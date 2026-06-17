using LunaticPanel.PackageManager.Domain.Entites;
using LunaticPanel.PackageManager.Domain.Respositories;

namespace LunaticPanel.PackageManager.Infrastructure.Repositories;

internal class SourceRepository : ISourceRepository
{
    public Task AddAsync(RepositorySourceEntity repositorySource, CancellationToken ct = default) => throw new NotImplementedException();
    public Task DisableAsync(RepositorySourceEntity repositorySource, CancellationToken ct = default) => throw new NotImplementedException();
    public Task EnableAsync(RepositorySourceEntity repositorySource, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<IEnumerable<RepositorySourceEntity>> GetAllAsync(CancellationToken ct = default) => throw new NotImplementedException();
    public Task RemoveAsync(RepositorySourceEntity repositorySource, CancellationToken ct = default) => throw new NotImplementedException();
}
