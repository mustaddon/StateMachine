using System;
using System.Threading.Tasks;

namespace FluentStateMachine;

public static class FsmConfigExtensions
{
    public static FsmStateConfig<TState, TEvent> OnEnter<TState, TEvent>(this FsmStateConfig<TState, TEvent> cfg,
        Action<IFsmEnterArgs<TState, TEvent>> action)
        => cfg.OnEnter(x => { action(x); return Task.CompletedTask; });

    public static FsmStateConfig<TState, TEvent> OnExit<TState, TEvent>(this FsmStateConfig<TState, TEvent> cfg,
        Action<IFsmExitArgs<TState, TEvent>> action)
         => cfg.OnExit(x => { action(x); return Task.CompletedTask; });

    public static FsmStateConfig<TState, TEvent> Enable<TState, TEvent>(this FsmStateConfig<TState, TEvent> cfg,
        Func<IFsmEnterArgs<TState, TEvent>, bool> fn)
        => cfg.Enable(x => Task.FromResult(fn(x)));



    public static FsmEventConfig<TState, TEvent, TData> Execute<TState, TEvent, TData, TResult>(this FsmEventConfig<TState, TEvent, TData> cfg,
        Func<IFsmTriggerArgs<TState, TEvent, TData>, TResult> fn)
        => cfg.Execute(x => Task.FromResult(fn(x)));

    public static FsmEventConfig<TState, TEvent, TData> Execute<TState, TEvent, TData>(this FsmEventConfig<TState, TEvent, TData> cfg,
        Action<IFsmTriggerArgs<TState, TEvent, TData>> action)
        => cfg.Execute(x => { action(x); return Task.CompletedTask; });

    public static FsmEventConfig<TState, TEvent, TData> Enable<TState, TEvent, TData>(this FsmEventConfig<TState, TEvent, TData> cfg,
        Func<IFsmTriggerArgs<TState, TEvent, TData>, bool> fn)
        => cfg.Enable(x => Task.FromResult(fn(x)));

    public static FsmEventConfig<TState, TEvent, TData> JumpTo<TState, TEvent, TData>(this FsmEventConfig<TState, TEvent, TData> cfg,
        Func<IFsmTriggerArgs<TState, TEvent, TData>, TState> fn)
        => cfg.JumpTo(x => Task.FromResult(fn(x)));

    public static FsmEventConfig<TState, TEvent, TData> JumpTo<TState, TEvent, TData>(this FsmEventConfig<TState, TEvent, TData> cfg,
        TState state)
        => cfg.JumpTo(x => Task.FromResult(state));



    public static FsmEventConfig<TState, TEvent, object> On<TState, TEvent>(this FsmBuilder<TState, TEvent> cfg, TEvent e)
        => cfg.On<object>(e);

    public static FsmEventConfig<TState, TEvent, object> On<TState, TEvent>(this FsmStateConfig<TState, TEvent> cfg, TEvent e)
        => cfg.On<object>(e);

    public static FsmEventConfig<TState, TEvent, object> On<TState, TEvent, T>(this FsmEventConfig<TState, TEvent, T> cfg, TEvent e)
        => cfg.On<object>(e);



    public static FsmEventConfig<TState, TEvent, TData> OnX<TState, TEvent, TData, TResult>(this FsmBuilder<TState, TEvent> cfg, IFsmEvent<TData, TResult> e)
        where TEvent : IFsmEvent
        => cfg.On<TData>((TEvent)e);

    public static FsmEventConfig<TState, TEvent, TData> OnX<TState, TEvent, TData, TResult>(this FsmStateConfig<TState, TEvent> cfg, IFsmEvent<TData, TResult> e)
        where TEvent : IFsmEvent
        => cfg.On<TData>((TEvent)e);

    public static FsmEventConfig<TState, TEvent, TData> OnX<TState, TEvent, T, TData, TResult>(this FsmEventConfig<TState, TEvent, T> cfg, IFsmEvent<TData, TResult> e)
        where TEvent : IFsmEvent
        => cfg.On<TData>((TEvent)e);
}