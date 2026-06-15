using LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;
using LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;
using MedihatR;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.PackageManager.Application.Mediator;

internal static class RegisterServiceExt
{
    public static void AddApplicationMediator(this IServiceCollection services)
    {
        services.AddMedihaterHandler<RepositorySourceRemoveHandler>();
        services.AddMedihaterHandler<RepositorySourceEnableHandler>();
        services.AddMedihaterHandler<RepositorySourceDisableHandler>();
        services.AddMedihaterHandler<RepositorySourceAddHandler>();
        services.AddMedihaterHandler<PackageUpdateHandler>();
        services.AddMedihaterHandler<PackageRemoveHandler>();
        services.AddMedihaterHandler<PackageInstallHandler>();
        services.AddMedihaterHandler<PackageEnableHandler>();
        services.AddMedihaterHandler<PackageDisableHandler>();
        services.AddMedihaterHandler<SearchPackagesHandler>();
        services.AddMedihaterHandler<SearchRepositoryHandler>();
        services.AddMedihaterHandler<GetAllPackagesHandler>();
        services.AddMedihaterHandler<GetPackagesLatestVersionHandler>();
        services.AddMedihaterHandler<GetPackageVersionsHandler>();
    }
}
