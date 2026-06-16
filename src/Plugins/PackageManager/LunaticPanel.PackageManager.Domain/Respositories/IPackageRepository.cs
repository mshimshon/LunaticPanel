using LunaticPanel.PackageManager.Domain.Entites;
using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;
using LunaticPanel.PackageManager.Domain.QueryModels.Interfaces;

namespace LunaticPanel.PackageManager.Domain.Respositories;

public interface IPackageRepository
{
    Task<IQueryModelResult<PackageInfo>> QueryAsync(IPackageQueryModel queryModel, CancellationToken ct = default);
    Task<PackageEntity> GetByIdAsync(PackageId id, CancellationToken ct = default);
    Task InstallAsync(PackageEntity package, CancellationToken ct = default);
    Task UpdateAsync(PackageEntity target, CancellationToken ct = default);
    Task DeleteAsync(PackageId id, CancellationToken ct = default);
    Task EnableAsync(PackageId id, CancellationToken ct = default);
    Task DisableAsync(PackageId id, CancellationToken ct = default);
    Task<ICollection<PackageEntity>> GetAll(CancellationToken ct = default);
}
