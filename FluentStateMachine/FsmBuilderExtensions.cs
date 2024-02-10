using System;
using System.Threading.Tasks;

namespace FluentStateMachine;

public static class FsmBuilderExtensions
{
    public static FsmBuilder<TState, TEvent> OnReset<TState, TEvent>(this FsmBuilder<TState, TEvent> builder,
        Action<IFsmResetArgs<TState, TEvent>> action)
    {
        return builder.OnReset(x => { action(x); return Task.CompletedTask; });
    }

    public static FsmBuilder<TState, TEvent> OnExit<TState, TEvent>(this FsmBuilder<TState, TEvent> builder,
        Action<IFsmExitArgs<TState, TEvent>> action)
    {
        return builder.OnExit(x => { action(x); return Task.CompletedTask; });
    }

    public static FsmBuilder<TState, TEvent> OnEnter<TState, TEvent>(this FsmBuilder<TState, TEvent> builder,
        Action<IFsmEnterArgs<TState, TEvent>> action)
    {
        return builder.OnEnter(x => { action(x); return Task.CompletedTask; });
    }

    public static FsmBuilder<TState, TEvent> OnJump<TState, TEvent>(this FsmBuilder<TState, TEvent> builder,
        Action<IFsmEnterArgs<TState, TEvent>> action)
    {
        return builder.OnJump(x => { action(x); return Task.CompletedTask; });
    }

    public static FsmBuilder<TState, TEvent> OnTrigger<TState, TEvent>(this FsmBuilder<TState, TEvent> builder,
        Action<IFsmTriggerArgs<TState, TEvent>> action)
    {
        return builder.OnTrigger(x => { action(x); return Task.CompletedTask; });
    }

    public static FsmBuilder<TState, TEvent> OnFire<TState, TEvent>(this FsmBuilder<TState, TEvent> builder,
        Action<IFsmTriggerArgs<TState, TEvent>> action)
    {
        return builder.OnFire(x => { action(x); return Task.CompletedTask; });
    }

    public static FsmBuilder<TState, TEvent> OnComplete<TState, TEvent>(this FsmBuilder<TState, TEvent> builder,
        Action<IFsmCompleteArgs<TState, TEvent>> action)
    {
        return builder.OnComplete(x => { action(x); return Task.CompletedTask; });
    }

    public static FsmBuilder<TState, TEvent> OnError<TState, TEvent>(this FsmBuilder<TState, TEvent> builder,
        Action<IFsmErrorArgs<TState, TEvent>> action)
    {
        return builder.OnError(x => { action(x); return Task.CompletedTask; });
    }

}
