using LunaticPanel.Engine.Application.Circuit;
using LunaticPanel.Engine.Infrastructure;
using LunaticPanel.Engine.Presentation.Layout.Menu.ViewModels;
using LunaticPanel.Engine.Presentation.Services;
using LunaticPanel.Engine.Presentation.Services.Messaging;
using MudBlazor;
using MudBlazor.Services;
using StatePulse.Net;

namespace LunaticPanel.Engine.Presentation;

public static class RegisterServicesExt
{
    public static IServiceCollection AddLunaticPanelServices(this IServiceCollection services)
    {
        services.AddEngineInfrastructure();

        services.AddScoped<MainMenuViewModel>();

        services.AddScoped<CircuitRegistry>();
        services.AddScoped<ICircuitControl, CircuitRegistry>();
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
            o.DispatchEffectBehavior = StatePulse.Net.Configuration.DispatchEffectBehavior.Parallel;
            o.DispatchEffectExecutionBehavior = StatePulse.Net.Configuration.DispatchEffectExecutionBehavior.YieldAndFire;
            o.DispatchOrderBehavior = StatePulse.Net.Configuration.DispatchOrdering.ReducersFirst;
            o.ScanAssemblies = [
                typeof(RegisterServicesExt),
                ];
        });

        services.ScanBusHandlers();
        foreach (var item in services.ScanBusHandlers())
            services.AddTransient(item.HandlerType);

        return services;
    }
}
