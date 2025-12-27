using LunaticPanel.Core.Messaging.EngineBus;
using LunaticPanel.Engine.Infrastructure;
using LunaticPanel.Engine.Infrastructure.Circuit;
using LunaticPanel.Engine.Services;
using LunaticPanel.Engine.Services.Messaging;
using LunaticPanel.Engine.Services.Messaging.EngineBus;
using LunaticPanel.Engine.Services.Messaging.EventBus;
using LunaticPanel.Engine.Services.Messaging.QueryBus;
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
        services.AddScoped<IEngineBus, EngineBusService>();
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

        services.ScanEngineBusHandlers([typeof(RegisterServicesExt).Assembly]);
        services.ScanEventBusHandlers([typeof(RegisterServicesExt).Assembly]);
        services.ScanQueryBusHandlers([typeof(RegisterServicesExt).Assembly]);
        return services;
    }

    public static IServiceCollection ScanBusMessagingHandlersFor(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.ScanEngineBusHandlers(assemblies);
        services.ScanEventBusHandlers(assemblies);
        services.ScanQueryBusHandlers(assemblies);
        return services;
    }
    public static IApplicationBuilder AddAdditionalAssemblies(this IApplicationBuilder builder, params Assembly[] assemblies)
    {
        Routes.AdditionalAssemblies.AddRange(assemblies);
        return builder;
    }

    public static WebApplication UseLunaticPanelEngine(this WebApplication webApplication)
    {
        webApplication.RegisterScannedEngineBusHandlers();
        webApplication.RegisterScannedEventBusHandlers();
        webApplication.RegisterScannedQueryBusHandlers();
        return webApplication;
    }
}
