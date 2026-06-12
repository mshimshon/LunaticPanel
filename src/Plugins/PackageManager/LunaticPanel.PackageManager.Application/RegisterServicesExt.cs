using LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;
using LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;
using MedihatR;
using Microsoft.Extensions.DependencyInjection;


namespace LunaticPanel.PackageManager.Application;

public static class RegisterServicesExt
{
    public static bool _medihaterScanned = false;
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddMedihaterServices();
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
    }
}
