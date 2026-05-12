using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.Engine.Application.Pulses.Actions;
using LunaticPanel.Engine.Application.Pulses.States;
using LunaticPanel.Engine.Web.Services.Circuit;
using StatePulse.Net;

namespace LunaticPanel.Engine.Infrastructure.Services;

internal class WidgetComponentLifecycle : IWidgetComponentLifecycle
{
    private readonly IDispatcher _dispatcher;
    private readonly CircuitRegistry _circuitRegistry;
    private readonly IStateAccessor<DisposalChainState> _disposalChainStateAccess;

    public WidgetComponentLifecycle(IDispatcher dispatcher, CircuitRegistry circuitRegistry, IStateAccessor<DisposalChainState> disposalChainStateAccess)
    {
        _dispatcher = dispatcher;
        _circuitRegistry = circuitRegistry;
        _disposalChainStateAccess = disposalChainStateAccess;
    }
    public Task BringComponentAlive()
        => _dispatcher.Prepare<IncreaseComponentCountAction>().Await().DispatchAsync();
    public async Task KillComponent()
    {
        await _dispatcher.Prepare<DecreaseComponentCountAction>().Await().DispatchAsync();
        Console.WriteLine($"WidgetComponentLifecycle::DecreaseComponentCountAction");
        if (_disposalChainStateAccess.State.ComponentCount <= 0)
        {
            Console.WriteLine($"WidgetComponentLifecycle::Removal Now");
            _circuitRegistry.SelfRemoval();

        }
    }

}
