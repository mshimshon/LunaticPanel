using LunaticPanel.PackageManager.Domain.Entites;
using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

namespace LunaticPanel.PackageManager.Domain.Respositories;

public interface IPackageRepository
{
    PackageEntity GetByIdAsync(PackageId id, CancellationToken ct = default);
}
