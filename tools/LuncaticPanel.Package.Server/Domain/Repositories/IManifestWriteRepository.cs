using LuncaticPanel.Package.Server.Domain.Entites;
using LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;

namespace LuncaticPanel.Package.Server.Domain.Repositories;

public interface IManifestWriteRepository
{
    Task CreateAsync(ManifestEntity manifest, CancellationToken ct = default);
    Task HideAsync(PackageId id, PackageVersion version, CancellationToken ct = default);
    Task EndLifeAsync(PackageId id, PackageEndOfLifeMessage message, CancellationToken ct = default);
}
