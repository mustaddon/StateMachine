using System.Threading;

namespace FluentStateMachine;

public interface IFsmArgs<TState, TEvent>
{
    IStateMachine<TState, TEvent> Fsm { get; }
    CancellationToken CancellationToken { get; }
}

public interface IFsmDataArgs<TState, TEvent> : IFsmArgs<TState, TEvent>
{
    object Data { get; }
}

public interface IFsmResetArgs<TState, TEvent> : IFsmArgs<TState, TEvent>, IFsmPrevStateArg<TState>
{

}

public interface IFsmEnterArgs<TState, TEvent> : IFsmDataArgs<TState, TEvent>, IFsmPrevStateArg<TState>
{

}

public interface IFsmExitArgs<TState, TEvent> : IFsmDataArgs<TState, TEvent>
{
    TState NextState { get; }
}

public interface IFsmErrorArgs<TState, TEvent> : IFsmDataArgs<TState, TEvent>
{
    string Error { get; }
}

public interface IFsmTriggerArgs<TState, TEvent> : IFsmDataArgs<TState, TEvent>
{
    TEvent Event { get; }
}

public interface IFsmCompleteArgs<TState, TEvent> : IFsmTriggerArgs<TState, TEvent>, IFsmPrevStateArg<TState>
{
    object Result { get; }
}

public interface IFsmPrevStateArg<TState>
{
    TState PrevState { get; }
}