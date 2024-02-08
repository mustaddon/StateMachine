using System.Threading;

namespace FluentStateMachine;

public class FsmArgs<TState, TEvent>
{
    internal FsmArgs() { }
    public IStateMachine<TState, TEvent> Fsm { get; internal set; }
    public CancellationToken CancellationToken { get; internal set; }
}

public class FsmResetArgs<TState, TEvent> : FsmArgs<TState, TEvent>
{
    internal FsmResetArgs() { }
    public TState PrevState { get; internal set; }
}

public class FsmDataArgs<TState, TEvent> : FsmArgs<TState, TEvent>
{
    internal FsmDataArgs() { }
    public object Data { get; internal set; }
}

public class FsmExitArgs<TState, TEvent> : FsmDataArgs<TState, TEvent>
{
    internal FsmExitArgs() { }
    public TState NextState { get; internal set; }
}

public class FsmEnterArgs<TState, TEvent> : FsmDataArgs<TState, TEvent>
{
    internal FsmEnterArgs() { }
    public TState PrevState { get; internal set; }
}

public class FsmErrorArgs<TState, TEvent> : FsmDataArgs<TState, TEvent>
{
    internal FsmErrorArgs() { }
    public string Message { get; internal set; }
}

public class FsmTriggerArgs<TState, TEvent> : FsmDataArgs<TState, TEvent>
{
    internal FsmTriggerArgs() { }
    public TEvent Event { get; internal set; }
}

public class FsmCompleteArgs<TState, TEvent> : FsmTriggerArgs<TState, TEvent>
{
    internal FsmCompleteArgs() { }
    public object Result { get; internal set; }
}
