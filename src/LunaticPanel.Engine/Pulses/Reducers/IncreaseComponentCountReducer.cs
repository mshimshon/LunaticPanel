using LunaticPanel.Engine.Pulses.Actions;
using LunaticPanel.Engine.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.Engine.Pulses.Reducers;

internal sealed class IncreaseComponentCountReducer : IReducer<DisposalChainState, IncreaseComponentCountAction>
{
    public DisposalChainState Reduce(DisposalChainState state, IncreaseComponentCountAction action)
        => state with { ComponentCount = state.ComponentCount + 1 };
}
