using LunaticPanel.Core.Utils;
using LunaticPanel.Core.Utils.Abstraction.FileWatcher;
using LunaticPanel.Core.Utils.Abstraction.FileWatcher.Enums;
using LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework;
using LunaticPanel.Package.LocalServer.Infrastructure.Extensions;
using LunaticPanel.Package.LocalServer.Infrastructure.LunaPackage;
using LunaticPanel.Package.LocalServer.Infrastructure.Services.FileWatcher;
using LuncaticPanel.Package.Server.Application.Mediator.Commands;
using LuncaticPanel.Package.Server.Application.Mediator.Engine;
using LuncaticPanel.Package.Server.Application.Payloads.Enums;
using LuncaticPanel.Package.Server.Application.Payloads.Requests;
using LuncaticPanel.Package.Server.Application.Services;
using LuncaticPanel.Package.Server.Domain.Repositories;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace LunaticPanel.Package.LocalServer.Infrastructure;

public static class InfrastructureServiceRegisterExt
{
    internal static string PackageDatabaseLocation { get; set; } = OperatingSystem.IsLinux() ? "/var/lib/lunaticpanel_lpkg_localserver/db/local.db" : "local.db";
    internal static HashSet<string> PackageStorageBase { get; set; } = new(StringComparer.OrdinalIgnoreCase) {
        "/var/lib/lunaticpanel_lpkg_localserver/lpkgs"
    };

    internal static Dictionary<string, string> PackageStorageUpload { get; set; } = [];
    internal static Dictionary<string, string> PackageStorageServe { get; set; } = [];
    public static void AddLocalServerInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        if (!OperatingSystem.IsLinux())
            PackageDatabaseLocation = "local.db";
        else
        {
            PackageDatabaseLocation = configuration.GetSection("LunaPackage")?.GetValue<string>("Database") ?? "/var/lib/lunaticpanel_lpkg_localserver/db/local.db";

            string? dir = Path.GetDirectoryName(PackageDatabaseLocation);
            if (dir != default && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
        if (OperatingSystem.IsLinux())
        {
            var locationToWatch = configuration.GetSection("LunaPackage")?.GetValue<string[]>("StorageWatch") ?? Array.Empty<string>();
            foreach (var item in locationToWatch)
                PackageStorageBase.Add(item);
            foreach (var watch in PackageStorageBase)
            {
                var uploadLocation = Path.Combine(watch, "awaiting");
                var serveLocation = Path.Combine(watch, "available");
                if (!Directory.Exists(uploadLocation))
                    Directory.CreateDirectory(uploadLocation);
                PackageStorageUpload[watch] = uploadLocation;

                if (!Directory.Exists(serveLocation))
                    Directory.CreateDirectory(serveLocation);
                PackageStorageServe[watch] = serveLocation;
            }
        }
        services.AddHttpContextAccessor();
        services.AddFileWatcherFactoryUtilityService();
        services.AddTransient<IManifestReadRepository, ManifestReadRepository>();
        services.AddTransient<IManifestWriteRepository, ManifestWriteRepository>();
        services.AddTransient<IPackageDownloadResolver, PackageDownloadResolver>();
        services.AddDbContext<PackageDatabaseContext>(options =>
        {
            options.UseSqlite($"Data Source={PackageDatabaseLocation}");
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });
    }

    public static async Task UseLocalServerInfrastructure(this WebApplication app)
    {

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PackageDatabaseContext>();
        await db.Database.EnsureCreatedAsync();
        scope.Dispose();
        await app.UseLocalServerStaticServe();
        app.Services.StartLocationWatchers();

    }
    private static async Task UseLocalServerStaticServe(this WebApplication app)
    {
        var strictZipProvider = new FileExtensionContentTypeProvider();
        strictZipProvider.Mappings.Clear();
        strictZipProvider.Mappings.Add(".lpkg", "application/zip");
        foreach (var location in PackageStorageServe)
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(location.Value),
                ContentTypeProvider = strictZipProvider,
                // The URL path that clients will use to access the assets via HTTP/HTTPS
                RequestPath = $"/{location.Value.ToBase32()}"
            });
        // Scan awaiting folders to trigger startup validation.
        foreach (var toValidate in PackageStorageUpload)
        {
            Console.WriteLine($"Scanning: {toValidate.Value}");
            foreach (var file in Directory.GetFiles(toValidate.Value, "*.lpkg"))
            {
                Console.WriteLine($"Processing: {file}");

                using var scope = app.Services.CreateScope();
                try
                {
                    await scope.ServiceProvider.AutoCreateNewlyAddedPackageFiles(file);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine($"Deleting '{file}'");
                    File.Delete(file);
                    Console.WriteLine($"Deleted '{file}'");
                }
                finally
                {
                    scope.Dispose();
                }
            }
        }
    }
    private static void StartLocationWatchers(this IServiceProvider sp)
    {
        if (!OperatingSystem.IsLinux())
            return;
        using var scope = sp.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<IFileWatcherSystemFactory>();
        foreach (var location in PackageStorageUpload)
            factory.CreateFileWatchUsing<WatchLocation>(location.Value, "*.lpkg", [FileWatchEvent.Created, FileWatchEvent.Updated], OnWatchLocationChanged);
    }
    private static async Task OnWatchLocationChanged(WatchLocation watchLocation, IServiceProvider sp)
    {
        if (watchLocation.Event != FileWatchEvent.Created || watchLocation.Event != FileWatchEvent.Updated) return;
        if (watchLocation.FullName == default || !File.Exists(watchLocation.FullName)) return;
        using var scope = sp.CreateScope();
        await scope.ServiceProvider.AutoCreateNewlyAddedPackageFiles(watchLocation.FullName);
    }
    private static async Task AutoCreateNewlyAddedPackageFiles(this IServiceProvider sp, string file, CancellationToken ct = default)
    {
        Console.WriteLine($"{file} newly added.");
        var mediator = sp.GetRequiredService<IMediator>();

        var validationRequest = new PackageValidationRequest()
        {
            LocationType = PackageValidationLocation.Local,
            Target = file
        };
        Console.WriteLine($"Sending request for '{file}'.");
        var manifest = await mediator.ExecuteAsync(new CreateManifestCommand(validationRequest));
        Console.WriteLine($"{manifest.Id} {manifest.Version} was created ('{file}').");
        var baseDir = Path.GetDirectoryName(file);
        var serveLocation = Path.GetFullPath(Path.Combine(baseDir!, "../available"));
        var filename = $"{manifest.Id}.{manifest.Version}";
        var moveTo = Path.Combine(serveLocation, $"{filename.ToBase32()}.lpkg");
        Console.WriteLine($"'{file}' -> '{moveTo}'.");
        File.Move(file, moveTo);
        Console.WriteLine($"{file} moved and available '{moveTo}'.");
    }
}
