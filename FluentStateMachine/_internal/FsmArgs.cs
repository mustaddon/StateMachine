using System.Threading;

namespace FluentStateMachine._internal;

internal class FsmArgs<TState, TEvent>(StateMachine<TState, TEvent> fsm) : IFsmArgs<TState, TEvent>
{
    public IStateMachine<TState, TEvent> Fsm => fsm;
    public TState State => fsm.Current;
    public virtual TEvent Event => fsm.LastEvent;
    public CancellationToken CancellationToken { get; internal set; }
}

internal class FsmDataArgs<TState, TEvent>(StateMachine<TState, TEvent> fsm) : FsmArgs<TState, TEvent>(fsm), IFsmDataArgs<TState, TEvent, object>
{
    public object Data { get; internal set; }
}

internal class FsmErrorArgs<TState, TEvent>(StateMachine<TState, TEvent> fsm) : FsmDataArgs<TState, TEvent>(fsm), IFsmErrorArgs<TState, TEvent>
{
    public string Error { get; internal set; }
}

internal class FsmEnterArgs<TState, TEvent>(StateMachine<TState, TEvent> fsm) : FsmErrorArgs<TState, TEvent>(fsm), IFsmEnterArgs<TState, TEvent>
{
    public TState PrevState { get; internal set; }
}

internal class FsmJumpArgs<TState, TEvent>(StateMachine<TState, TEvent> fsm) : FsmEnterArgs<TState, TEvent>(fsm), IFsmExitArgs<TState, TEvent>
{
    public TState NextState { get; internal set; }
}

internal class FsmCompleteArgs<TState, TEvent>(StateMachine<TState, TEvent> fsm, TEvent e) : FsmErrorArgs<TState, TEvent>(fsm), IFsmCompleteArgs<TState, TEvent>
{
    public override TEvent Event { get; } = e;
    public TState PrevState { get; internal set; }
    public object Result { get; internal set; }
}

internal class FsmTriggerArgs<TState, TEvent>(StateMachine<TState, TEvent> fsm, TEvent e) : FsmDataArgs<TState, TEvent>(fsm), IFsmTriggerArgs<TState, TEvent>
{
    public override TEvent Event { get; } = e;
}