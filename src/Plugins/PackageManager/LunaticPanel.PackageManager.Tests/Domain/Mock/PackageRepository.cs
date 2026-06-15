using LunaticPanel.PackageManager.Domain.Entites;
using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;
using LunaticPanel.PackageManager.Domain.QueryModels.Interfaces;
using LunaticPanel.PackageManager.Domain.Respositories;
using LunaticPanel.PackageManager.Domain.Respositories.Exceptions;

namespace LunaticPanel.PackageManager.Tests.Domain.Mock;

internal class PackageRepository : IPackageRepository
{
    public List<PackageEntity> LocalPackages { get; set; } = new();

    public Task DeleteAsync(PackageId id, CancellationToken ct = default)
    {
        LocalPackages = LocalPackages.Where(p => p.Info.Id != id).ToList();
        return Task.CompletedTask;
    }

    public Task DisableAsync(PackageId id, CancellationToken ct = default)
    {
        var package = LocalPackages.SingleOrDefault(p => p.Info.Id == id);
        if (package == default) return Task.CompletedTask;
        LocalPackages.Remove(package);
        LocalPackages.Add(package with
        {
            Info = package.Info with
            {
                State = PackageManager.Domain.Entites.Enums.PackageState.Disabled
            }
        });
        return Task.CompletedTask;
    }

    public Task EnableAsync(PackageId id, CancellationToken ct = default)
    {
        var package = LocalPackages.SingleOrDefault(p => p.Info.Id == id);
        if (package == default) return Task.CompletedTask;
        LocalPackages.Remove(package);
        LocalPackages.Add(package with
        {
            Info = package.Info with
            {
                State = PackageManager.Domain.Entites.Enums.PackageState.Enabled
            }
        });
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

    public Task InstallAsync(PackageEntity package, CancellationToken ct = default)
    {
        LocalPackages.Add(package);
        return Task.CompletedTask;
    }

    public Task<IQueryModelResult<PackageInfo>> QueryAsync(IPackageQueryModel queryModel, CancellationToken ct = default)
        => throw new NotImplementedException();

    public Task UpdateAsync(PackageEntity current, PackageEntity target, CancellationToken ct = default)
    {
        var package = LocalPackages.SingleOrDefault(p => p.Info.Id == current.Info.Id);
        if (package == default) return Task.CompletedTask;
        LocalPackages.Remove(package);
        LocalPackages.Add(target with
        {
            Info = target.Info with
            {
                State = package.Info.State
            }
        });
        return Task.CompletedTask;
    }
}
