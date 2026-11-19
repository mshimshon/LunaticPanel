using StatePulse.Net;
using System;

namespace LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Application.Mock;

public class StateAccessor<TState> : IStateAccessor<TState> where TState : IStateFeature
{
    public static IStateAccessor<TState> CreateState()
    {
        var feature = Activator.CreateInstance<TState>();
        var type = typeof(StateAccessor<TState>);
        return (IStateAccessor<TState>)Activator.CreateInstance(type, feature)!;
    }

    public StateAccessor(TState state)
    {
        State = state;
    }

    public TState State { get; private set; }

    public StateVersioning Version => throw new NotImplementedException();

    public event OnChangeEventHandler<TState>? OnStateChanged;
    public event EventHandler? OnStateChangedNoDetails;

    public bool ChangeState(TState state, Type originType, long version, Guid dispatchWriter) => true;
    T IStateAccessor.GetAs<T>() => throw new NotImplementedException();
}
