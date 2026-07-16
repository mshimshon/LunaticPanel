using LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework;
using LunaticPanel.Package.LocalServer.Infrastructure.LunaPackage;
using LuncaticPanel.Package.Server.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LunaticPanel.Package.LocalServer.Infrastructure;

public static class InfrastructureServiceRegisterExt
{
    internal static string PackageDatabaseLocation { get; set; } = OperatingSystem.IsLinux() ? "/etc/lunaticpackage/local.db" : "local.db";
    public static void AddLocalServerInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IManifestReadRepository, ManifestReadRepository>();
        services.AddTransient<IManifestWriteRepository, ManifestWriteRepository>();
        services.AddDbContext<PackageDatabaseContext>(options =>
        {
            options.UseSqlite($"Data Source={PackageDatabaseLocation}");
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
    }

    public static void UseLocalServerInfrastructure(this WebApplication app)
    {

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PackageDatabaseContext>();
        db.Database.EnsureCreated();
    }
}
