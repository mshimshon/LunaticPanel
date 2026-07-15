using LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework;
using LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework.Models.Mapping;
using LuncaticPanel.Package.Server.Domain.Entites;
using LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;
using LuncaticPanel.Package.Server.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LunaticPanel.Package.LocalServer.Infrastructure.LunaPackage;

public class ManifestWriteRepository : IManifestWriteRepository
{
    private readonly PackageDatabaseContext _packageDatabase;

    public ManifestWriteRepository(PackageDatabaseContext packageDatabase)
    {
        _packageDatabase = packageDatabase;
    }
    public async Task CreateAsync(ManifestEntity manifest, CancellationToken ct = default)
    {
        await _packageDatabase.PackageVersions.AddAsync(manifest.ToPackageInfoModel(), ct);
        await _packageDatabase.SaveChangesAsync(ct);
    }
    public async Task EndLifeAsync(PackageId id, PackageEndOfLifeMessage message, CancellationToken ct = default)
    {
        var model = await _packageDatabase.Packages.AsSplitQuery().Include(p => p.Versions)
            .SingleOrDefaultAsync(p => p.Id == id.Value, ct);
        //TODO: THROW NOT FOUND

        model.EndOfLifeMessage = message.Value;

        _packageDatabase.Packages.Update(model);
        _packageDatabase.PackageVersions.UpdateRange(model.Versions.Select(p =>
        {
            p.Hidden = true;
            return p;
        }));
        await _packageDatabase.SaveChangesAsync(ct);
    }
    public async Task HideAsync(PackageId id, PackageVersion version, CancellationToken ct = default)
    {
        var model = await _packageDatabase.PackageVersions.SingleOrDefaultAsync(p => p.PackageId == id.Value && p.Version == version.Value, ct);
        //TODO: THROW NOT FOUND
        model.Hidden = true;
        _packageDatabase.PackageVersions.Update(model);
        await _packageDatabase.SaveChangesAsync(ct);
    }
}
