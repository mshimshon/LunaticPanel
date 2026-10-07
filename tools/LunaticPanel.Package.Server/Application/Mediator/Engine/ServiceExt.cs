using LunaticPanel.Package.Server.Application.Mediator.Commands;
using LunaticPanel.Package.Server.Application.Mediator.Commands.Handlers;
using LunaticPanel.Package.Server.Application.Mediator.Queries;
using LunaticPanel.Package.Server.Application.Mediator.Queries.Handlers;
using LunaticPanel.Package.Server.Application.Payloads;
using LunaticPanel.Package.Server.Application.Payloads.Responses;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Package.Server.Application.Mediator.Engine;

internal static class ServiceExt
{
    public static void AddMediatorServices(this IServiceCollection services)
    {
        services.AddScoped<IMediator, Mediator>();
        services.AddTransient<IRequestHandler<SearchManifestQuery, ManifestSearchResponse>, SearchManifestHandler>();
        services.AddTransient<IRequestHandler<GetAllPackageVersionsQuery, ICollection<ManifestPayload>>, GetAllPackageVersionsHandler>();
        services.AddTransient<IRequestHandler<GetLatestPackageQuery, ManifestPayload>, GetLatestPackageHandler>();
        services.AddTransient<IRequestHandler<GetSpecificPackageVersionQuery, ManifestPayload>, GetSpecificPackageVersionHandler>();
        services.AddTransient<IRequestHandler<CreateManifestCommand, ManifestPayload>, CreateManifestHandler>();
        services.AddTransient<IRequestHandler<HideManifestVersionCommand>, HideManifestVersionHandler>();
        services.AddTransient<IRequestHandler<EndManifestLifeCommand>, EndManifestLifeHandler>();
        services.AddTransient<IRequestHandler<PackageValidationCommand, PackageValidationResponse>, PackageValidationHandler>();
        services.AddTransient<IRequestHandler<SearchManifestQuery, ManifestSearchResponse>, SearchManifestHandler>();
        services.AddTransient<IRequestHandler<GetPackageDownloadTargetQuery, PackageDownloadTargetResponse>, GetPackageDownloadTargetHandler>();
    }
}
