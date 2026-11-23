using StatePulse.Net;

namespace LunaticPanel.Engine.Pulses.States;

public sealed record DisposalChainState : IStateFeature
{
    public int ComponentCount { get; init; }

}
