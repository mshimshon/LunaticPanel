using LunaticPanel.Engine.Application.Pulses.Actions;
using LunaticPanel.Engine.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.Engine.Application.Pulses.Reducers;

internal sealed class IncreaseComponentCountReducer : IReducer<DisposalChainState, IncreaseComponentCountAction>
{
    public DisposalChainState Reduce(DisposalChainState state, IncreaseComponentCountAction action)
        => state with { ComponentCount = state.ComponentCount + 1 };
}
