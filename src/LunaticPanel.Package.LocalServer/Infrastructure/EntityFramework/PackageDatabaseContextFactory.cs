using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework;

internal class PackageDatabaseContextFactory : IDesignTimeDbContextFactory<PackageDatabaseContext>
{
    public PackageDatabaseContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PackageDatabaseContext>();
        optionsBuilder.UseSqlite("Data Source=/etc/lunaticpackage/local.db");
        var db = new PackageDatabaseContext(optionsBuilder.Options);
        return db;
    }

}
