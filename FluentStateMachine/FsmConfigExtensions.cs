﻿using System;
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



    public static FsmEventConfig<TState, TEvent, TData, object> Execute<TState, TEvent, TData>(this FsmEventConfig<TState, TEvent, TData, object> cfg,
        Func<IFsmTriggerArgs<TState, TEvent, TData>, Task> fn)
        => cfg.Execute(fn);

    public static FsmEventConfig<TState, TEvent, TData, object> Execute<TState, TEvent, TData>(this FsmEventConfig<TState, TEvent, TData, object> cfg,
        Action<IFsmTriggerArgs<TState, TEvent, TData>> action)
        => cfg.Execute(x => { action(x); return Task.CompletedTask; });

    public static FsmEventConfig<TState, TEvent, TData, TResult> Execute<TState, TEvent, TData, TResult>(this FsmEventConfig<TState, TEvent, TData, TResult> cfg,
        Func<IFsmTriggerArgs<TState, TEvent, TData>, TResult> fn)
        => cfg.Execute(x => Task.FromResult(fn(x)));

    public static FsmEventConfig<TState, TEvent, TData, TResult> Enable<TState, TEvent, TData, TResult>(this FsmEventConfig<TState, TEvent, TData, TResult> cfg,
        Func<IFsmTriggerArgs<TState, TEvent, TData>, bool> fn)
        => cfg.Enable(x => Task.FromResult(fn(x)));

    public static FsmEventConfig<TState, TEvent, TData, TResult> JumpTo<TState, TEvent, TData, TResult>(this FsmEventConfig<TState, TEvent, TData, TResult> cfg,
        Func<IFsmTriggerArgs<TState, TEvent, TData>, TState> fn)
        => cfg.JumpTo(x => Task.FromResult(fn(x)));

    public static FsmEventConfig<TState, TEvent, TData, TResult> JumpTo<TState, TEvent, TData, TResult>(this FsmEventConfig<TState, TEvent, TData, TResult> cfg,
        TState state)
        => cfg.JumpTo(x => Task.FromResult(state));



    public static FsmEventConfig<TState, TEvent, object, object> On<TState, TEvent>(this FsmBuilder<TState, TEvent> cfg, TEvent e)
        => cfg.On<object, object>(e);

    public static FsmEventConfig<TState, TEvent, object, object> On<TState, TEvent>(this FsmStateConfig<TState, TEvent> cfg, TEvent e)
        => cfg.On<object, object>(e);

    public static FsmEventConfig<TState, TEvent, object, object> On<TState, TEvent, TData, TResult>(this FsmEventConfig<TState, TEvent, TData, TResult> cfg, TEvent e)
        => cfg.On<object, object>(e);



    public static FsmEventConfig<TState, TEvent, TData, TResult> OnX<TState, TEvent, TData, TResult>(this FsmBuilder<TState, TEvent> cfg, IFsmEvent<TData, TResult> e)
        where TEvent : IFsmEvent
        => cfg.On<TData, TResult>((TEvent)e);

    public static FsmEventConfig<TState, TEvent, TData, TResult> OnX<TState, TEvent, TData, TResult>(this FsmStateConfig<TState, TEvent> cfg, IFsmEvent<TData, TResult> e)
        where TEvent : IFsmEvent
        => cfg.On<TData, TResult>((TEvent)e);

    public static FsmEventConfig<TState, TEvent, TArgsData, TExecuteResult> OnX<TState, TEvent, TData, TResult, TArgsData, TExecuteResult>(this FsmEventConfig<TState, TEvent, TData, TResult> cfg, IFsmEvent<TArgsData, TExecuteResult> e)
        where TEvent : IFsmEvent
        => cfg.On<TArgsData, TExecuteResult>((TEvent)e);
}