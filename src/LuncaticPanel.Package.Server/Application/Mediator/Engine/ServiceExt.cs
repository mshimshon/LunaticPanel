using LuncaticPanel.Package.Server.Application.Mediator.Queries;
using LuncaticPanel.Package.Server.Application.Mediator.Queries.Handlers;
using LuncaticPanel.Package.Server.Application.Payloads;
using LuncaticPanel.Package.Server.Application.Payloads.Responses;
using Microsoft.Extensions.DependencyInjection;

namespace LuncaticPanel.Package.Server.Application.Mediator.Engine;

internal static class ServiceExt
{
    public static void AddMediatorServices(this IServiceCollection services)
    {
        services.AddTransient<IMediator, Mediator>();
        services.AddTransient<IRequestHandler<SearchManifestQuery, ManifestSearchResponse>, SearchManifestHandler>();
        services.AddTransient<IRequestHandler<GetAllPackageVersionsQuery, ICollection<ManifestPayload>>, GetAllPackageVersionsHandler>();
        services.AddTransient<IRequestHandler<GetLatestPackageQuery, ManifestPayload>, GetLatestPackageHandler>();
        services.AddTransient<IRequestHandler<GetSpecificPackageVersionQuery, ManifestPayload>, GetSpecificPackageVersionHandler>();
    }
}
