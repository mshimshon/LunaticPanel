using LunaticPanel.Package.Server.Domain.Entites;
using LunaticPanel.Package.Server.Domain.Entites.ValueObjects;
using LunaticPanel.Package.Server.Domain.Query;

namespace LunaticPanel.Package.Server.Domain.Repositories;

public interface IManifestReadRepository
{
    Task<IManifestQueryResultModel> SearchAsync(IManifestQueryModel q, CancellationToken ct = default);
    Task<ManifestEntity> GetMostRecentAsync(PackageId id, CancellationToken ct = default);
    Task<ManifestEntity> GetAsync(PackageId id, PackageVersion version, CancellationToken ct = default);
    Task<ICollection<ManifestEntity>> GetAllAsync(PackageId id, CancellationToken ct = default);
    Task<bool> ExistAsync(PackageId id, CancellationToken ct = default);
    Task<bool> ExistAsync(PackageId id, PackageVersion version, CancellationToken ct = default);
}
