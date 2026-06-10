using LunaticPanel.PackageManager.Domain.Entites;
using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;
using LunaticPanel.PackageManager.Domain.QueryModels.Interfaces;

namespace LunaticPanel.PackageManager.Domain.Respositories;

public interface IPackageRepository
{
    Task<IQueryModelResult<PackageInfo>> QueryAsync(IPackageQueryModel queryModel, CancellationToken ct = default);
    PackageEntity GetByIdAsync(PackageId id, CancellationToken ct = default);
    Task<ICollection<PackageInfo>> GetInstalledAsync(CancellationToken ct = default);
    Task InstallAsync(PackageId id, PackageVersion version, CancellationToken ct = default);
    Task UpdateAsync(PackageId id, PackageVersion currentVersion, PackageVersion targetVersion, CancellationToken ct = default);
    Task DeleteAsync(PackageId id, CancellationToken ct = default);
    Task EnableAsync(PackageId id, CancellationToken ct = default);
    Task DisableAsync(PackageId id, CancellationToken ct = default);
}
