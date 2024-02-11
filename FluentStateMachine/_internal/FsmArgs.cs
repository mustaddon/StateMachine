﻿using System.Threading;

namespace FluentStateMachine._internal;

internal class FsmArgs<TState, TEvent> : IFsmArgs<TState, TEvent>
{
    public IStateMachine<TState, TEvent> Fsm { get; internal set; }
    public CancellationToken CancellationToken { get; internal set; }
}

internal class FsmResetArgs<TState, TEvent> : FsmArgs<TState, TEvent>, IFsmResetArgs<TState, TEvent>
{
    public TState PrevState { get; internal set; }
}

internal class FsmDataArgs<TState, TEvent> : FsmArgs<TState, TEvent>, IFsmDataArgs<TState, TEvent>
{
    public object Data { get; internal set; }
}

internal class FsmErrorArgs<TState, TEvent> : FsmDataArgs<TState, TEvent>, IFsmErrorArgs<TState, TEvent>
{
    public string Error { get; internal set; }
}

internal class FsmEnterExitArgs<TState, TEvent> : FsmErrorArgs<TState, TEvent>, IFsmEnterArgs<TState, TEvent>, IFsmExitArgs<TState, TEvent>
{
    public TState PrevState { get; internal set; }
    public TState NextState { get; internal set; }
}

internal class FsmCompleteArgs<TState, TEvent> : FsmErrorArgs<TState, TEvent>, IFsmCompleteArgs<TState, TEvent>
{
    public TEvent Event { get; internal set; }
    public TState PrevState { get; internal set; }
    public object Result { get; internal set; }
}

internal class FsmTriggerArgs<TState, TEvent> : FsmDataArgs<TState, TEvent>, IFsmTriggerArgs<TState, TEvent>
{
    public TEvent Event { get; internal set; }
}