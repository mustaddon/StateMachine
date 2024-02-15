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



    public static FsmEventConfig<TState, TEvent, TData> Execute<TState, TEvent, TData, TResult>(this FsmEventConfig<TState, TEvent, TData> cfg,
        Func<IFsmTriggerArgs<TState, TEvent, TData>, TResult> fn)
    {
        return cfg.Execute(x => Task.FromResult(fn(x)));
    }

    public static FsmEventConfig<TState, TEvent, TData> Execute<TState, TEvent, TData>(this FsmEventConfig<TState, TEvent, TData> cfg,
        Action<IFsmTriggerArgs<TState, TEvent, TData>> action)
    {
        return cfg.Execute(x => { action(x); return Task.CompletedTask; });
    }

    public static FsmEventConfig<TState, TEvent, TData> Enable<TState, TEvent, TData>(this FsmEventConfig<TState, TEvent, TData> cfg,
        Func<IFsmTriggerArgs<TState, TEvent, TData>, bool> fn)
    {
        return cfg.Enable(x => Task.FromResult(fn(x)));
    }

    public static FsmEventConfig<TState, TEvent, TData> JumpTo<TState, TEvent, TData>(this FsmEventConfig<TState, TEvent, TData> cfg,
        TState state)
    {
        return cfg.JumpTo(x => Task.FromResult(state));
    }

    public static FsmEventConfig<TState, TEvent, TData> JumpTo<TState, TEvent, TData>(this FsmEventConfig<TState, TEvent, TData> cfg,
        Func<IFsmTriggerArgs<TState, TEvent, TData>, TState> fn)
    {
        return cfg.JumpTo(x => Task.FromResult(fn(x)));
    }



    public static FsmEventConfig<TState, TEvent, object> On<TState, TEvent>(this FsmBuilder<TState, TEvent> cfg, TEvent e)
    {
        return cfg.On<object>(e);
    }

    public static FsmEventConfig<TState, TEvent, object> On<TState, TEvent>(this FsmStateConfig<TState, TEvent> cfg, TEvent e)
    {
        return cfg.On<object>(e);
    }

    public static FsmEventConfig<TState, TEvent, object> On<TState, TEvent, T>(this FsmEventConfig<TState, TEvent, T> cfg, TEvent e)
    {
        return cfg.On<object>(e);
    }



    public static FsmEventConfig<TState, TEvent, TData> OnX<TState, TEvent, TData, TResult>(this FsmBuilder<TState, TEvent> cfg, IFsmEvent<TData, TResult> e)
        where TEvent : IFsmEvent
    {
        return cfg.On<TData>((TEvent)e);
    }

    public static FsmEventConfig<TState, TEvent, object> OnX<TState, TEvent>(this FsmBuilder<TState, TEvent> cfg, IFsmEvent e)
        where TEvent : IFsmEvent
    {
        return cfg.On<object>((TEvent)e);
    }



    public static FsmEventConfig<TState, TEvent, TData> OnX<TState, TEvent, TData, TResult>(this FsmStateConfig<TState, TEvent> cfg, IFsmEvent<TData, TResult> e)
        where TEvent : IFsmEvent
    {
        return cfg.On<TData>((TEvent)e);
    }

    public static FsmEventConfig<TState, TEvent, object> OnX<TState, TEvent>(this FsmStateConfig<TState, TEvent> cfg, IFsmEvent e)
        where TEvent : IFsmEvent
    {
        return cfg.On<object>((TEvent)e);
    }



    public static FsmEventConfig<TState, TEvent, TData> OnX<TState, TEvent, T, TData, TResult>(this FsmEventConfig<TState, TEvent, T> cfg, IFsmEvent<TData, TResult> e)
        where TEvent : IFsmEvent
    {
        return cfg.On<TData>((TEvent)e);
    }

    public static FsmEventConfig<TState, TEvent, object> OnX<TState, TEvent, T>(this FsmEventConfig<TState, TEvent, T> cfg, IFsmEvent e)
        where TEvent : IFsmEvent
    {
        return cfg.On<object>((TEvent)e);
    }
}