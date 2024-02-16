using System.Threading;

namespace FluentStateMachine;

public interface IFsmArgs<TState, TEvent>
{
    IStateMachine<TState, TEvent> Fsm { get; }
    CancellationToken CancellationToken { get; }
    TState State { get; }
    TEvent Event { get; }
}

public interface IFsmDataArgs<TState, TEvent, TData> : IFsmArgs<TState, TEvent>
{
    TData Data { get; }
}

public interface IFsmEnterArgs<TState, TEvent> : IFsmDataArgs<TState, TEvent, object>, IFsmPrevStateArg<TState>;

public interface IFsmExitArgs<TState, TEvent> : IFsmDataArgs<TState, TEvent, object>
{
    TState NextState { get; }
}

public interface IFsmErrorArgs<TState, TEvent> : IFsmDataArgs<TState, TEvent, object>
{
    string Error { get; }
}

public interface IFsmTriggerArgs<TState, TEvent> : IFsmTriggerArgs<TState, TEvent, object>;

public interface IFsmTriggerArgs<TState, TEvent, TData> : IFsmDataArgs<TState, TEvent, TData>;

public interface IFsmCompleteArgs<TState, TEvent> : IFsmTriggerArgs<TState, TEvent>, IFsmPrevStateArg<TState>
{
    object Result { get; }
}

public interface IFsmPrevStateArg<TState>
{
    TState PrevState { get; }
}