using LunaticPanel.Core.Utils;
using LunaticPanel.Core.Utils.Abstraction.FileWatcher;
using LunaticPanel.Core.Utils.Abstraction.FileWatcher.Enums;
using LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework;
using LunaticPanel.Package.LocalServer.Infrastructure.LunaPackage;
using LunaticPanel.Package.LocalServer.Infrastructure.Services.FileWatcher;
using LuncaticPanel.Package.Server.Application.Mediator.Commands;
using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Application.Payloads.Enums;
using LuncaticPanel.Package.Server.Application.Payloads.Requests;
using LuncaticPanel.Package.Server.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LunaticPanel.Package.LocalServer.Infrastructure;

public static class InfrastructureServiceRegisterExt
{
    internal static string PackageDatabaseLocation { get; set; } = OperatingSystem.IsLinux() ? "/var/lib/lunaticpanel_package/db/local.db" : "local.db";
    internal static string[] PackageStorageWatch { get; set; } = [
        "/var/lib/lunaticpanel_package/lpkgs"
    ];
    public static void AddLocalServerInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        if (!OperatingSystem.IsLinux())
            PackageDatabaseLocation = "local.db";
        else
        {
            PackageDatabaseLocation = configuration.GetSection("LunaPackage")?.GetValue<string>("Database") ?? "/var/lib/lunaticpanel_package/db/local.db";

            string? dir = Path.GetDirectoryName(PackageDatabaseLocation);
            if (dir != default && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
        if (OperatingSystem.IsLinux())
        {
            var locationToWatch = configuration.GetSection("LunaPackage")?.GetValue<string[]>("StorageWatch") ?? Array.Empty<string>();
            PackageStorageWatch = PackageStorageWatch.Union(locationToWatch).ToArray();
        }

        services.AddFileWatcherFactoryUtilityService();
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
        scope.ServiceProvider.StartLocationWatchers();
    }
    private static void StartLocationWatchers(this IServiceProvider sp)
    {
        var factory = sp.GetRequiredService<IFileWatcherSystemFactory>();
        foreach (var location in PackageStorageWatch)
            factory.CreateFileWatchUsing<WatchLocation>(location, "*.lpkg", [FileWatchEvent.Created, FileWatchEvent.Updated], OnWatchLocationChanged);
    }
    private static async Task OnWatchLocationChanged(WatchLocation watchLocation, IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        if (watchLocation.FullName == default) return;
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var validationRequest = new PackageValidationRequest()
        {
            LocationType = PackageValidationLocation.Local,
            Target = watchLocation.FullName
        };
        await mediator.ExecuteAsync(new CreateManifestCommand(validationRequest));
    }
}
