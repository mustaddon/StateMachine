using System;
using System.Threading.Tasks;

namespace FluentStateMachine;

public static class FsmConfigExtensions
{
    public static FsmStateConfig<TState, TEvent> OnEnter<TState, TEvent>(this FsmStateConfig<TState, TEvent> cfg,
        Action<IFsmEnterArgs<TState, TEvent>> action)
    {
        return cfg.OnEnter(x => { action(x); return Task.CompletedTask; });
    }

    public static FsmStateConfig<TState, TEvent> OnExit<TState, TEvent>(this FsmStateConfig<TState, TEvent> cfg,
        Action<IFsmExitArgs<TState, TEvent>> action)
    {
        return cfg.OnExit(x => { action(x); return Task.CompletedTask; });
    }

    public static FsmStateConfig<TState, TEvent> Enable<TState, TEvent>(this FsmStateConfig<TState, TEvent> cfg,
        Func<IFsmEnterArgs<TState, TEvent>, bool> fn)
    {
        return cfg.Enable(x => Task.FromResult(fn(x)));
    }


    public static FsmEventConfig<TState, TEvent> Execute<TState, TEvent>(this FsmEventConfig<TState, TEvent> cfg,
        Func<IFsmTriggerArgs<TState, TEvent>, object> fn)
    {
        return cfg.Execute(x => Task.FromResult(fn(x)));
    }

    public static FsmEventConfig<TState, TEvent> Enable<TState, TEvent>(this FsmEventConfig<TState, TEvent> cfg,
        Func<IFsmTriggerArgs<TState, TEvent>, bool> fn)
    {
        return cfg.Enable(x => Task.FromResult(fn(x)));
    }

    public static FsmEventConfig<TState, TEvent> JumpTo<TState, TEvent>(this FsmEventConfig<TState, TEvent> cfg,
        TState state)
    {
        return cfg.JumpTo(x => Task.FromResult(state));
    }

    public static FsmEventConfig<TState, TEvent> JumpTo<TState, TEvent>(this FsmEventConfig<TState, TEvent> cfg,
        Func<IFsmTriggerArgs<TState, TEvent>, TState> fn)
    {
        return cfg.JumpTo(x => Task.FromResult(fn(x)));
    }
}
