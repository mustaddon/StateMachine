using System;
using FluentStateMachine._internal;

namespace FluentStateMachine;

public static class FsmBuilderExtensions
{
    public static FsmBuilder<TState, TEvent> OnReset<TState, TEvent>(this FsmBuilder<TState, TEvent> builder,
        Action<FsmResetArgs<TState, TEvent>> action)
    {
        return builder.OnReset(x => { action(x); return FrameworkExt.CompletedTask; });
    }

    public static FsmBuilder<TState, TEvent> OnExit<TState, TEvent>(this FsmBuilder<TState, TEvent> builder,
        Action<FsmExitArgs<TState, TEvent>> action)
    {
        return builder.OnExit(x => { action(x); return FrameworkExt.CompletedTask; });
    }

    public static FsmBuilder<TState, TEvent> OnEnter<TState, TEvent>(this FsmBuilder<TState, TEvent> builder,
        Action<FsmEnterArgs<TState, TEvent>> action)
    {
        return builder.OnEnter(x => { action(x); return FrameworkExt.CompletedTask; });
    }

    public static FsmBuilder<TState, TEvent> OnJump<TState, TEvent>(this FsmBuilder<TState, TEvent> builder,
        Action<FsmEnterArgs<TState, TEvent>> action)
    {
        return builder.OnJump(x => { action(x); return FrameworkExt.CompletedTask; });
    }

    public static FsmBuilder<TState, TEvent> OnTrigger<TState, TEvent>(this FsmBuilder<TState, TEvent> builder,
        Action<FsmTriggerArgs<TState, TEvent>> action)
    {
        return builder.OnTrigger(x => { action(x); return FrameworkExt.CompletedTask; });
    }

    public static FsmBuilder<TState, TEvent> OnFire<TState, TEvent>(this FsmBuilder<TState, TEvent> builder,
        Action<FsmTriggerArgs<TState, TEvent>> action)
    {
        return builder.OnFire(x => { action(x); return FrameworkExt.CompletedTask; });
    }

    public static FsmBuilder<TState, TEvent> OnComplete<TState, TEvent>(this FsmBuilder<TState, TEvent> builder,
        Action<FsmCompleteArgs<TState, TEvent>> action)
    {
        return builder.OnComplete(x => { action(x); return FrameworkExt.CompletedTask; });
    }

    public static FsmBuilder<TState, TEvent> OnError<TState, TEvent>(this FsmBuilder<TState, TEvent> builder,
        Action<FsmErrorArgs<TState, TEvent>> action)
    {
        return builder.OnError(x => { action(x); return FrameworkExt.CompletedTask; });
    }

}
