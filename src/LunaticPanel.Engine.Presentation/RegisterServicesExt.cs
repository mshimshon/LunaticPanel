global using Microsoft.Extensions.DependencyInjection;
using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.EngineBus;
using LunaticPanel.Core.Abstraction.Messaging.EventBus;
using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;
using LunaticPanel.Core.Messaging;
using LunaticPanel.Core.Messaging.EngineBus;
using LunaticPanel.Core.Messaging.EventBus;
using LunaticPanel.Core.Messaging.QuerySystem;
using LunaticPanel.Engine.Infrastructure;
using LunaticPanel.Engine.Web.Layout.Menu;
using LunaticPanel.Engine.Web.Pages.Dashboard;
using LunaticPanel.Engine.Web.Services.Circuit;
using MedihatR;
using MudBlazor;
using MudBlazor.Services;
using StatePulse.Net;
namespace LunaticPanel.Engine.Web;

public static class RegisterServicesExt
{
    private readonly static EngineBusRegistry _engineBusRegistry = new();
    private readonly static EventBusRegistry _eventBusRegistry = new();
    private readonly static QueryBusRegistry _queryBusRegistry = new();
    public static IServiceCollection AddLunaticPanelServices(this IServiceCollection services)
    {
        services.AddEngineInfrastructure();

        services.AddScoped<MainMenuViewModel>();
        services.AddScoped<DashboardViewModel>();

        services.AddScoped<CircuitRegistry>();
        services.AddScoped<ICircuitRegistry>((sp) => sp.GetRequiredService<CircuitRegistry>());
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
            o.PulseTrackingPerformance = StatePulse.Net.Configuration.PulseTrackingModel.BlazorServerSafe;

            o.ScanAssemblies = [
                    typeof(RegisterServicesExt).Assembly,
                    typeof(Application.RegisterServicesExt).Assembly,
                    typeof(Infrastructure.RegisterServicesExt).Assembly,
                ];
        });
        services.AddMedihaterServices(o =>
        {
            o.AssembliesScan = [
                typeof(RegisterServicesExt),
                typeof(Application.RegisterServicesExt),
                typeof(Infrastructure.RegisterServicesExt),
             ];
        });

        services.AddSingleton<IEngineBusRegistry>((sp) => _engineBusRegistry);
        services.AddSingleton<IEventBusRegistry>((sp) => _eventBusRegistry);
        services.AddSingleton<IQueryBusRegistry>((sp) => _queryBusRegistry);
        var toRegisterBusHandlers = BusScannerExt.ScanBusHandlers(typeof(RegisterServicesExt).Assembly);
        foreach (var item in toRegisterBusHandlers)
        {
            services.AddTransient(item.HandlerType);
            if (item.BusType == EBusType.EventBus)
                _eventBusRegistry.Register(item.Id, item);
            else if (item.BusType == EBusType.QueryBus)
                _queryBusRegistry.Register(item.Id, item);
            else
                _engineBusRegistry.Register(item.Id, item);
        }

        return services;
    }
}
