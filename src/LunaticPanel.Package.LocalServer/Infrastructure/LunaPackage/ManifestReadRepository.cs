using LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework;
using LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework.Models.Mapping;
using LuncaticPanel.Package.Server.Domain.Entites;
using LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;
using LuncaticPanel.Package.Server.Domain.Query;
using LuncaticPanel.Package.Server.Domain.QueryModels;
using LuncaticPanel.Package.Server.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LunaticPanel.Package.LocalServer.Infrastructure.LunaPackage;

public class ManifestReadRepository : IManifestReadRepository
{
    private readonly PackageDatabaseContext _packageDatabase;

    public ManifestReadRepository(PackageDatabaseContext packageDatabase)
    {
        _packageDatabase = packageDatabase;
    }
    public Task<bool> ExistAsync(PackageId id, CancellationToken ct = default)
        => _packageDatabase.Packages.AnyAsync(p => p.Id == id.Value, ct);
    public Task<bool> ExistAsync(PackageId id, PackageVersion version, CancellationToken ct = default)
        => _packageDatabase.PackageVersions.AnyAsync(p => p.PackageId == id.Value && p.Version == version.Value, ct);
    public async Task<ICollection<ManifestEntity>> GetAllAsync(PackageId id, CancellationToken ct = default)
    {
        var result = await _packageDatabase.Packages
            .AsSplitQuery()
            .Include(p => p.Versions)
            .SingleOrDefaultAsync(p => p.Id == id.Value, ct);
        //TODO: TRHOW CODED EXCEPTION
        return result.Versions.Select(p => p.ToDomain()).ToList();
    }
    public async Task<ManifestEntity> GetAsync(PackageId id, PackageVersion version, CancellationToken ct = default)
    {
        var result = await _packageDatabase.PackageVersions
            .AsSplitQuery()
            .Include(p => p.Package)
            .SingleOrDefaultAsync(p => p.PackageId == id.Value && p.Version == version.Value, ct);
        //TODO: TRHOW CODED EXCEPTION
        return result.ToDomain();
    }
    public async Task<ManifestEntity> GetMostRecentAsync(PackageId id, CancellationToken ct = default)
    {
        var latest = _packageDatabase.PackageVersions
            .Where(p => p.PackageId == id.Value)
            .ToList()
            .OrderByDescending(p => Version.Parse(p.Version))
            .FirstOrDefault();

        //TODO: TRHOW CODED EXCEPTION
        return latest.ToDomain();
    }
    public Task<IManifestQueryResultModel> SearchAsync(IManifestQueryModel q, CancellationToken ct = default)
        => throw new NotImplementedException();
}
