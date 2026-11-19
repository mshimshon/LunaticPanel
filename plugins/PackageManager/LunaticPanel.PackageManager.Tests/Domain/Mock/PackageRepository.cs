using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Domain.Entities.ValueObjects;
using LunaticPanel.PackageManager.Domain.QueryModels;
using LunaticPanel.PackageManager.Domain.QueryModels.Interfaces;
using LunaticPanel.PackageManager.Domain.Respositories;
using LunaticPanel.PackageManager.Domain.Respositories.Exceptions;

namespace LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Domain.Mock;

internal class PackageRepository : IPackageRepository
{
    public List<PackageEntity> LocalPackages { get; set; } = new();

    public Task DeleteAsync(PackageId id, CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }

    public Task DisableAsync(PackageId id, CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }

    public Task EnableAsync(PackageId id, CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }

    public Task<ICollection<PackageEntity>> GetAll(CancellationToken ct = default)
    {
        ICollection<PackageEntity> result = LocalPackages.ToList();
        return Task.FromResult(result);
    }

    public Task<PackageEntity> GetByIdAsync(PackageId id, CancellationToken ct = default)
    {
        var package = LocalPackages.SingleOrDefault(p => p.Info.Id == id);
        if (package == default) throw new PackageNotFoundException(id);
        return Task.FromResult(package);
    }

    public Task InstallAsync(PackageEntity package, CancellationToken ct = default) => Task.CompletedTask;

    public async Task<IQueryModelResult<PackageInfo>> QueryAsync(IPackageQueryModel queryModel, CancellationToken ct = default)
    {
        var package = await GetAll(ct);
        var result = new QueryModelResult<PackageInfo>()
        {
            Position = 0,
            Result = package.Select(p => p.Info).ToList(),
            Total = package.Count
        };
        return result;
    }

    public Task UpdateAsync(PackageEntity current, PackageEntity target, CancellationToken ct = default) => Task.CompletedTask;

    public Task UpdateAsync(PackageEntity target, CancellationToken ct = default) => Task.CompletedTask;
}
