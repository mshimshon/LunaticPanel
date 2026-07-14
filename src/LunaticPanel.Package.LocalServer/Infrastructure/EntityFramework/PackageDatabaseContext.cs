using LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework.ModelConfiguration;
using LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework;

public class PackageDatabaseContext : DbContext
{
    public DbSet<PackageModel> Packages { get; set; } = null!;
    public DbSet<PackageInfoModel> PackageVersions { get; set; } = null!;

    public PackageDatabaseContext(DbContextOptions<PackageDatabaseContext> options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PackageModelConfiguration());
        modelBuilder.ApplyConfiguration(new PackageInfoModelConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
