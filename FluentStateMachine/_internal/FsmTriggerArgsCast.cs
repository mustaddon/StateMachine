using System.Threading;

namespace FluentStateMachine._internal;

internal class FsmTriggerArgsCast<TState, TEvent, TData>(IFsmTriggerArgs<TState, TEvent> args) : IFsmTriggerArgs<TState, TEvent, TData>
{

    readonly IFsmTriggerArgs<TState, TEvent> _args = args;

    public IStateMachine<TState, TEvent> Fsm => _args.Fsm;
    public CancellationToken CancellationToken => _args.CancellationToken;
    public TState State => _args.State;
    public TEvent Event => _args.Event;
    public TData Data => (TData)_args.Data;
}

