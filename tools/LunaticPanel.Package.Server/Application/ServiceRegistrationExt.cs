using LunaticPanel.Package.Server.Application.Mediator.Commands.Handlers;
using LunaticPanel.Package.Server.Application.Mediator.Queries.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Package.Server.Application;

public static class ServiceRegistrationExt
{
    internal static void AddApplicationLayerServices(this IServiceCollection services)
    {
        services.AddTransient<SearchManifestHandler>();
        services.AddTransient<GetAllPackageVersionsHandler>();
        services.AddTransient<GetLatestPackageHandler>();
        services.AddTransient<GetSpecificPackageVersionHandler>();
        services.AddTransient<CreateManifestHandler>();
        services.AddTransient<HideManifestVersionHandler>();
        services.AddTransient<EndManifestLifeHandler>();
        services.AddTransient<PackageValidationHandler>();
        services.AddTransient<SearchManifestHandler>();
        services.AddTransient<GetPackageDownloadTargetHandler>();
        services.AddTransient<PackageUploadHandler>();
        services.AddTransient<GetUploadAccessHandler>();
    }

}
