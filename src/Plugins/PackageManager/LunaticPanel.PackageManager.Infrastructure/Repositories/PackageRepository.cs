using LunaticPanel.PackageManager.Domain.Entites;
using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;
using LunaticPanel.PackageManager.Domain.QueryModels.Interfaces;
using LunaticPanel.PackageManager.Domain.Respositories;

namespace LunaticPanel.PackageManager.Infrastructure.Repositories;

internal class PackageRepository : IPackageRepository
{

    public Task DeleteAsync(PackageId id, CancellationToken ct = default) => throw new NotImplementedException();
    public Task DisableAsync(PackageId id, CancellationToken ct = default) => throw new NotImplementedException();
    public Task EnableAsync(PackageId id, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<ICollection<PackageEntity>> GetAll(CancellationToken ct = default) => throw new NotImplementedException();
    public Task<PackageEntity> GetByIdAsync(PackageId id, CancellationToken ct = default) => throw new NotImplementedException();
    public Task InstallAsync(PackageEntity package, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<IQueryModelResult<PackageInfo>> QueryAsync(IPackageQueryModel queryModel, CancellationToken ct = default) => throw new NotImplementedException();
    public Task UpdateAsync(PackageEntity target, CancellationToken ct = default) => throw new NotImplementedException();
}
