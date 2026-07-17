using LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework;
using LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework.Exceptions;
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
        if (result == default)
            throw new PackageNotFoundException(id.Value);
        return result.Versions.Select(p => p.ToDomain()).ToList();
    }
    public async Task<ManifestEntity> GetAsync(PackageId id, PackageVersion version, CancellationToken ct = default)
    {
        var result = await _packageDatabase.PackageVersions
            .AsSplitQuery()
            .Include(p => p.Package)
            .SingleOrDefaultAsync(p => p.PackageId == id.Value && p.Version == version.Value, ct);
        if (result == default)
            throw new PackageVersionNotFoundException(id.Value, version.Value);
        return result.ToDomain();
    }
    public async Task<ManifestEntity> GetMostRecentAsync(PackageId id, CancellationToken ct = default)
    {
        var latest = _packageDatabase.PackageVersions
            .Where(p => p.PackageId == id.Value)
            .ToList()
            .OrderByDescending(p => Version.Parse(p.Version))
            .FirstOrDefault();
        if (latest == default)
            throw new PackageNotFoundException(id.Value);
        return latest.ToDomain();
    }

    public async Task<IManifestQueryResultModel> SearchAsync(IManifestQueryModel q, CancellationToken ct = default)
    {
        var query = _packageDatabase.PackageVersions.Where(p =>
        p.Version ==
            _packageDatabase.PackageVersions
                .Where(x => x.PackageId == p.PackageId)
                .OrderByDescending(x => x.Version)
                .Select(x => x.Version)
                .FirstOrDefault()
    );

        if (!q.ShowEndOfLife)
            query = (IOrderedQueryable<EntityFramework.Models.PackageInfoModel>)query.Where(p => p.Package.EndOfLifeMessage == default);
        if (!q.ShowHidden)
            query = query.Where(p => !p.Hidden);
        if (q.PanelVersion != default)
            query = query.Where(p => p.PanelVersion == q.PanelVersion.Value);
        if (q.Id != default)
            query = query.Where(p => p.PackageId == q.Id.Value);
        if (q.Keywords != default)
        {
            var lowered = q.Keywords.Value.Select(k => k.ToLower()).ToList();
            query = query.Where(p =>
                lowered.Any(k => p.PackageId.ToLower().Contains(k))
            );
        }
        var result = new ManifestQueryResultModel()
        {
            Position = q.Position,
            Total = query.Count()
        };
        var searchResult = await query.Skip(q.Position).Take(q.MaxResult).ToListAsync(ct);

        result = result with
        {
            Result = searchResult.Select(p => p.ToDomain()).ToList()
        }
        ;
        return result;
    }
}
