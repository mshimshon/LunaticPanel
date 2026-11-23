using LunaticPanel.Engine.Pulses.Actions;
using LunaticPanel.Engine.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.Engine.Pulses.Reducers;

internal sealed class DecreaseComponentCountReducer : IReducer<DisposalChainState, DecreaseComponentCountAction>
{
    public DisposalChainState Reduce(DisposalChainState state, DecreaseComponentCountAction action)
             => state with { ComponentCount = state.ComponentCount - 1 <= 0 ? 0 : state.ComponentCount - 1 };

}
