using LunaticPanel.Core;
using LunaticPanel.Core.Messaging.EngineBus;
using LunaticPanel.Engine.Infrastructure;
using LunaticPanel.Engine.Infrastructure.Circuit;
using LunaticPanel.Engine.Services;
using LunaticPanel.Engine.Services.Messaging;
using LunaticPanel.Engine.Services.Messaging.EngineBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using StatePulse.Net;
using SwizzleV;
using System.Reflection;
namespace LunaticPanel.Engine;

public static class RegisterServicesExt
{
    public static void SealServiceCollection(this IServiceCollection services)
    {
        HostServiceStorage.HostServices = services.ToList().AsReadOnly();

    }
    public static IServiceCollection AddLunaticPanelEngine(this IServiceCollection services)
    {
        services.AddLunaticPanelEngineInfrastructure();
        services.AddScoped<CircuitRegistry>();
        services.AddScoped<ICircuitControl, CircuitRegistry>();
        services.AddSingleton<EngineBusRegistry>();
        services.AddScoped<IEngineBus, EngineBus>();
        services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 10000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        });

        services.AddStatePulseServices(o =>
        {
            o.ScanAssemblies = [
                typeof(RegisterServicesExt),
                typeof(Application.RegisterServicesExt),
                typeof(Infrastructure.RegisterServicesExt)
                ];
        });


        services.AddSwizzleV();
        services.ScanBusHandlers();
        return services;
    }
    public static IServiceCollection AddPluginsFrom(this IServiceCollection services, string location)
    {

        return services;
    }

    public static IServiceCollection ScanBusHandlersFor(this IServiceCollection services, params IPlugin[] plugins)
    {
        foreach (var item in plugins)
            services.ScanBusHandlers(item);
        return services;
    }

    public static IApplicationBuilder AddAdditionalAssemblies(this IApplicationBuilder builder, params Assembly[] assemblies)
    {
        Routes.AdditionalAssemblies.AddRange(assemblies);
        return builder;
    }

    public static WebApplication UseLunaticPanelEngine(this WebApplication webApplication)
    {
        webApplication.RegisterScannedBusHandlers();
        return webApplication;
    }
}
