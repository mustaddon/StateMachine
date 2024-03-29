﻿using FluentStateMachine._internal;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FluentStateMachine;

public static class IStateMachineExtensions
{
    public static async Task<bool> TryJumpToAsync<TState>(this IStateController<TState> fsm, TState state, object data = null, CancellationToken cancellationToken = default)
    {
        if (fsm is IConcurrentStateController<TState> concurrent)
            return await concurrent.TryJumpToAsync(state, data, cancellationToken);

        if (!await fsm.IsAvailableStateAsync(state, data, cancellationToken))
            return false;

        await fsm.JumpToAsync(state, data, cancellationToken);
        return true;
    }

    public static bool TryJumpTo<TState>(this IStateController<TState> fsm, TState state, object data = null)
        => TryJumpToAsync(fsm, state, data).Result;

    public static void JumpTo<TState>(this IStateController<TState> fsm, TState state, object data = null)
        => fsm.JumpToAsync(state, data).Wait();



    public static void ResetTo<TState>(this IStateController<TState> fsm, TState state)
        => fsm.ResetToAsync(state).Wait();

    public static void Reset(this IStateController fsm)
        => fsm.ResetAsync().Wait();



    public static bool IsAvailableState<TState>(this IStateController<TState> fsm, TState value, object data = null)
        => fsm.IsAvailableStateAsync(value, data).Result;

    public static bool IsAvailableEvent<TEvent>(this IEventController<TEvent> fsm, TEvent value, object data = null)
        => fsm.IsAvailableEventAsync(value, data).Result;

    public static ICollection<TState> GetAvailableStates<TState>(this IStateController<TState> fsm, object data = null)
        => fsm.GetAvailableStatesAsync(data).Result;

    public static ICollection<TEvent> GetAvailableEvents<TEvent>(this IEventController<TEvent> fsm, object data = null)
        => fsm.GetAvailableEventsAsync(data).Result;



    public static async Task<ICollection<TState>> GetAvailableStatesAsync<TState>(this IStateController<TState> fsm, object data = null, CancellationToken cancellationToken = default)
    {
        var result = new HashSet<TState>();

        foreach (var value in fsm.States)
            if (await fsm.IsAvailableStateAsync(value, data, cancellationToken))
                result.Add(value);

        return result;
    }

    public static async Task<ICollection<TEvent>> GetAvailableEventsAsync<TEvent>(this IEventController<TEvent> fsm, object data = null, CancellationToken cancellationToken = default)
    {
        var result = new HashSet<TEvent>();

        foreach (var value in fsm.Events)
            if (await fsm.IsAvailableEventAsync(value, data, cancellationToken))
                result.Add(value);

        return result;
    }



    public static async Task<TResult> TriggerIfAvailableAsync<TEvent, TResult>(this IEventController<TEvent> fsm, TEvent e, object data = default, CancellationToken cancellationToken = default)
        => fsm is IConcurrentEventController<TEvent> concurrent ? await concurrent.TriggerIfAvailableAsync<TResult>(e, data, cancellationToken)
            : (await fsm.IsAvailableEventAsync(e, data, cancellationToken) ? await fsm.TriggerAsync<TResult>(e, data, cancellationToken) : default);

    public static TResult Trigger<TEvent, TResult>(this IEventController<TEvent> fsm, TEvent e, object data = null)
        => fsm.TriggerAsync<TResult>(e, data).Result;
    public static TResult TriggerIfAvailable<TEvent, TResult>(this IEventController<TEvent> fsm, TEvent e, object data = null)
        => TriggerIfAvailableAsync<TEvent, TResult>(fsm, e, data).Result;


    public static Task<object> TriggerAsync<TEvent>(this IEventController<TEvent> fsm, TEvent e, object data = default, CancellationToken cancellationToken = default)
        => fsm.TriggerAsync<object>(e, data, cancellationToken);
    public static Task<object> TriggerIfAvailableAsync<TEvent>(this IEventController<TEvent> fsm, TEvent e, object data = default, CancellationToken cancellationToken = default)
        => TriggerIfAvailableAsync<TEvent, object>(fsm, e, data, cancellationToken);

    public static object Trigger<TEvent>(this IEventController<TEvent> fsm, TEvent e, object data = null)
        => fsm.TriggerAsync<object>(e, data).Result;
    public static object TriggerIfAvailable<TEvent>(this IEventController<TEvent> fsm, TEvent e, object data = null)
        => TriggerIfAvailableAsync<TEvent, object>(fsm, e, data).Result;



    public static Task<TResult> TriggerAsync<TResult>(this IEventController<Type> fsm, IFsmEvent<TResult> e, CancellationToken cancellationToken = default)
        => fsm.TriggerAsync<TResult>(e.GetType(), e, cancellationToken);
    public static Task<TResult> TriggerIfAvailableAsync<TResult>(this IEventController<Type> fsm, IFsmEvent<TResult> e, CancellationToken cancellationToken = default)
        => TriggerIfAvailableAsync<Type, TResult>(fsm, e.GetType(), e, cancellationToken);


    public static TResult Trigger<TResult>(this IEventController<Type> fsm, IFsmEvent<TResult> e)
        => fsm.TriggerAsync<TResult>(e.GetType(), e).Result;
    public static TResult TriggerIfAvailable<TResult>(this IEventController<Type> fsm, IFsmEvent<TResult> e)
        => TriggerIfAvailableAsync<Type, TResult>(fsm, e.GetType(), e).Result;


    public static Task TriggerAsync(this IEventController<Type> fsm, IFsmEvent e, CancellationToken cancellationToken = default)
        => fsm.TriggerAsync<object>(e.GetType(), e, cancellationToken);
    public static Task TriggerIfAvailableAsync(this IEventController<Type> fsm, IFsmEvent e, CancellationToken cancellationToken = default)
        => TriggerIfAvailableAsync<Type, object>(fsm, e.GetType(), e, cancellationToken);


    public static void Trigger(this IEventController<Type> fsm, IFsmEvent e)
        => fsm.TriggerAsync<object>(e.GetType(), e).Wait();
    public static void TriggerIfAvailable(this IEventController<Type> fsm, IFsmEvent e)
        => TriggerIfAvailableAsync<Type, object>(fsm, e.GetType(), e).Wait();


    public static Task<TResult> TriggerAsync<TResult>(this IEventController<Type> fsm, object e, CancellationToken cancellationToken = default)
        => fsm.TriggerAsync<TResult>(e.GetType(), e, cancellationToken);
    public static Task<TResult> TriggerIfAvailableAsync<TResult>(this IEventController<Type> fsm, object e, CancellationToken cancellationToken = default)
        => TriggerIfAvailableAsync<Type, TResult>(fsm, e.GetType(), e, cancellationToken);


    public static TResult Trigger<TResult>(this IEventController<Type> fsm, object e)
        => fsm.TriggerAsync<TResult>(e.GetType(), e).Result;
    public static TResult TriggerIfAvailable<TResult>(this IEventController<Type> fsm, object e)
        => TriggerIfAvailableAsync<Type, TResult>(fsm, e.GetType(), e).Result;


    public static Task<object> TriggerAsync(this IEventController<Type> fsm, object e, CancellationToken cancellationToken = default)
        => fsm.TriggerAsync<object>(e.GetType(), e, cancellationToken);
    public static Task<object> TriggerIfAvailableAsync(this IEventController<Type> fsm, object e, CancellationToken cancellationToken = default)
        => TriggerIfAvailableAsync<Type, object>(fsm, e.GetType(), e, cancellationToken);


    public static object Trigger(this IEventController<Type> fsm, object e)
        => fsm.TriggerAsync<object>(e.GetType(), e).Result;
    public static object TriggerIfAvailable(this IEventController<Type> fsm, object e)
        => TriggerIfAvailableAsync<Type, object>(fsm, e.GetType(), e).Result;



    public static Task<TResult> TriggerAsync<TEvent, TResult>(this IEventController<TEvent> fsm, IFsmEvent<TResult> e, object data = default, CancellationToken cancellationToken = default)
        where TEvent : IFsmEventBase
        => fsm.TriggerAsync<TResult>((TEvent)e, data, cancellationToken);
    public static Task<TResult> TriggerIfAvailableAsync<TEvent, TResult>(this IEventController<TEvent> fsm, IFsmEvent<TResult> e, object data = default, CancellationToken cancellationToken = default)
        where TEvent : IFsmEventBase
        => TriggerIfAvailableAsync<TEvent, TResult>(fsm, (TEvent)e, data, cancellationToken);


    public static TResult Trigger<TEvent, TResult>(this IEventController<TEvent> fsm, IFsmEvent<TResult> e, object data = default)
        where TEvent : IFsmEventBase
        => fsm.TriggerAsync<TResult>((TEvent)e, data).Result;
    public static TResult TriggerIfAvailable<TEvent, TResult>(this IEventController<TEvent> fsm, IFsmEvent<TResult> e, object data = default)
        where TEvent : IFsmEventBase
        => TriggerIfAvailableAsync<TEvent, TResult>(fsm, (TEvent)e, data).Result;
    

    public static Task TriggerAsync<TEvent>(this IEventController<TEvent> fsm, IFsmEvent e, object data = default, CancellationToken cancellationToken = default)
        where TEvent : IFsmEventBase
        => fsm.TriggerAsync<object>((TEvent)e, data, cancellationToken);
    public static Task TriggerIfAvailableAsync<TEvent>(this IEventController<TEvent> fsm, IFsmEvent e, object data = default, CancellationToken cancellationToken = default)
        where TEvent : IFsmEventBase
        => TriggerIfAvailableAsync<TEvent, object>(fsm, (TEvent)e, data, cancellationToken);


    public static void Trigger<TEvent>(this IEventController<TEvent> fsm, IFsmEvent e, object data = default)
        where TEvent : IFsmEventBase
        => fsm.TriggerAsync<object>((TEvent)e, data).Wait();
    public static void TriggerIfAvailable<TEvent>(this IEventController<TEvent> fsm, IFsmEvent e, object data = default)
        where TEvent : IFsmEventBase
        => TriggerIfAvailableAsync<TEvent, object>(fsm, (TEvent)e, data).Wait();
}
