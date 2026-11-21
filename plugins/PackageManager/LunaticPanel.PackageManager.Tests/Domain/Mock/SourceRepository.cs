using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Domain.Respositories;

namespace LunaticPanel.PackageManager.Tests.Domain.Mock;

internal class SourceRepository : ISourceRepository
{

    public List<RepositorySourceEntity> Cache { get; set; } = new();
    public Task AddAsync(RepositorySourceEntity repositorySource, CancellationToken ct = default)
    {
        if (Cache.Contains(repositorySource))
            return Task.CompletedTask;
        Cache.Add(repositorySource);
        return Task.CompletedTask;

    }
    public Task DisableAsync(RepositorySourceEntity repositorySource, CancellationToken ct = default)
    {
        var source = Cache.SingleOrDefault(p => p == repositorySource);
        if (source == default) return Task.CompletedTask;
        Cache.Remove(source);
        Cache.Add(source with { State = PackageManager.Domain.Entities.Enums.RepositorySourceState.Disabled });
        return Task.CompletedTask;
    }
    public Task EnableAsync(RepositorySourceEntity repositorySource, CancellationToken ct = default)
    {
        var source = Cache.SingleOrDefault(p => p == repositorySource);
        if (source == default) return Task.CompletedTask;
        Cache.Remove(source);
        Cache.Add(source with { State = PackageManager.Domain.Entities.Enums.RepositorySourceState.Enabled });
        return Task.CompletedTask;
    }
    public Task<IEnumerable<RepositorySourceEntity>> GetAllAsync(CancellationToken ct = default)
    {
        IEnumerable<RepositorySourceEntity> result = Cache.ToList();
        return Task.FromResult(result);
    }
    public Task RemoveAsync(RepositorySourceEntity repositorySource, CancellationToken ct = default)
    {
        var source = Cache.SingleOrDefault(p => p == repositorySource);
        if (source == default) return Task.CompletedTask;
        Cache.Remove(source);
        return Task.CompletedTask;
    }
}
