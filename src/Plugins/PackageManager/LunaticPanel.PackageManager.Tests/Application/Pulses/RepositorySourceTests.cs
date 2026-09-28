using LunaticPanel.PackageManager.Application.Mediator;
using LunaticPanel.PackageManager.Application.Pulses;
using LunaticPanel.PackageManager.Application.Pulses.States;
using MedihatR;
using Microsoft.Extensions.DependencyInjection;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Tests.Application.Pulses;

internal class RepositorySourceTests
{
    private readonly IServiceProvider _scope;
    private readonly IDispatcher _dispatcher;
    private readonly IStateAccessor<RepositorySourceState> _state;
    public RepositorySourceTests()
    {
        IServiceCollection services = new ServiceCollection();
        // TODO: Register Mocked Services.
        // 
        services.AddStatePulseServices();
        services.AddMedihaterServices();
        services.AddApplicationPulses();
        services.AddApplicationMediator();
        _scope = services.BuildServiceProvider().CreateScope().ServiceProvider;
        _dispatcher = _scope.GetRequiredService<IDispatcher>();
        _state = _scope.GetRequiredService<IStateAccessor<RepositorySourceState>>();
    }


}
