using LuncaticPanel.Package.Server.Domain.Entites;
using LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;
using LuncaticPanel.Package.Server.Domain.Repositories;

namespace LunaticPanel.Package.LocalServer.Infrastructure.LunaPackage;

public class ManifestWriteRepository : IManifestWriteRepository
{
    public Task CreateAsync(ManifestEntity manifest, CancellationToken ct = default) => throw new NotImplementedException();
    public Task EndLifeAsync(PackageId id, PackageEndOfLifeMessage message, CancellationToken ct = default) => throw new NotImplementedException();
    public Task HideAsync(PackageId id, PackageVersion version, CancellationToken ct = default) => throw new NotImplementedException();
}
