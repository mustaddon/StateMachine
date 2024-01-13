using System;
using System.Threading.Tasks;
using FluentStateMachine._internal;

namespace FluentStateMachine;

public static class FsmConfigExtensions
{
    public static FsmStateConfig<TState, TEvent> OnEnter<TState, TEvent>(this FsmStateConfig<TState, TEvent> cfg,
        Action<FsmEnterArgs<TState, TEvent>> action)
    {
        return cfg.OnEnter(x => { action(x); return FrameworkExt.CompletedTask; });
    }

    public static FsmStateConfig<TState, TEvent> OnExit<TState, TEvent>(this FsmStateConfig<TState, TEvent> cfg,
        Action<FsmExitArgs<TState, TEvent>> action)
    {
        return cfg.OnExit(x => { action(x); return FrameworkExt.CompletedTask; });
    }

    public static FsmStateConfig<TState, TEvent> Enable<TState, TEvent>(this FsmStateConfig<TState, TEvent> cfg,
        Func<FsmEnterArgs<TState, TEvent>, bool> fn)
    {
        return cfg.Enable(x => Task.FromResult(fn(x)));
    }



    public static FsmEventConfig<TState, TEvent> Execute<TState, TEvent>(this FsmEventConfig<TState, TEvent> cfg,
        Func<FsmTriggerArgs<TState, TEvent>, object> fn)
    {
        return cfg.Execute(x => Task.FromResult(fn(x)));
    }

    public static FsmEventConfig<TState, TEvent> Enable<TState, TEvent>(this FsmEventConfig<TState, TEvent> cfg,
        Func<FsmTriggerArgs<TState, TEvent>, bool> fn)
    {
        return cfg.Enable(x => Task.FromResult(fn(x)));
    }

    public static FsmEventConfig<TState, TEvent> JumpTo<TState, TEvent>(this FsmEventConfig<TState, TEvent> cfg,
        TState state)
    {
        return cfg.JumpTo(x => Task.FromResult(state));
    }

    public static FsmEventConfig<TState, TEvent> JumpTo<TState, TEvent>(this FsmEventConfig<TState, TEvent> cfg,
        Func<FsmTriggerArgs<TState, TEvent>, TState> fn)
    {
        return cfg.JumpTo(x => Task.FromResult(fn(x)));
    }
}
