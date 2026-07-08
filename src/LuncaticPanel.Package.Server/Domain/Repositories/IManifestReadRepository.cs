using LuncaticPanel.Package.Server.Domain.Entites;
using LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;
using LuncaticPanel.Package.Server.Domain.Query;
using LuncaticPanel.Package.Server.Domain.QueryModels;

namespace LuncaticPanel.Package.Server.Domain.Repositories;

public interface IManifestReadRepository
{
    Task<IManifestQueryResultModel> SearchAsync(IManifestQueryModel q, CancellationToken ct = default);
    Task<ManifestEntity> GetAsync(PackageId id, CancellationToken ct = default);
    Task<ManifestEntity> GetAsync(PackageId id, PackageVersion version, CancellationToken ct = default);
    Task<ICollection<ManifestEntity>> GetAllAsync(PackageId id, CancellationToken ct = default);
    Task<bool> ExistAsync(PackageId id, CancellationToken ct = default);
    Task<bool> ExistAsync(PackageId id, PackageVersion version, CancellationToken ct = default);
}
