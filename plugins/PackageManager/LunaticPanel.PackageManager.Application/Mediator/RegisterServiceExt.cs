using LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;
using LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;
using MaksimShimshon.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.PackageManager.Application.Mediator;

internal static class RegisterServiceExt
{
    public static void AddApplicationMediator(this IServiceCollection services)
    {
        services.AddScoped<GetDiskPackagesHandler>();
        services.AddScoped<RepositorySourceRemoveHandler>();
        services.AddScoped<RepositorySourceEnableHandler>();
        services.AddScoped<RepositorySourceDisableHandler>();
        services.AddScoped<RepositorySourceAddHandler>();
        services.AddScoped<PackageUpdateHandler>();
        services.AddScoped<PackageRemoveHandler>();
        services.AddScoped<PackageInstallHandler>();
        services.AddScoped<PackageEnableHandler>();
        services.AddScoped<PackageDisableHandler>();
        services.AddScoped<SearchPackagesHandler>();
        services.AddScoped<SearchRepositoryHandler>();
        services.AddScoped<GetPackagesLatestVersionHandler>();
        services.AddScoped<GetPackageVersionsHandler>();
        services.AddScoped<GetSourcesHandler>();
        services.AddScoped<DisableSourceCommandHandler>();
        services.AddScoped<EnableSourceCommandHandler>();
        services.AddScoped<MoveDownSourceCommandHandler>();
        services.AddScoped<MoveUpSourceCommandHandler>();
        services.AddScoped<PeriodicCheckPackageUpdatesQueryHandler>();
        services.AddScoped<AutoUpdatePackagesCommandHandler>();
        services.AddScoped<PackageCancelInstallCommandHandler>();
        services.AddScoped<PackageRollbackCommandHandler>();
        // AI APPEND ABOVE
    }
}
