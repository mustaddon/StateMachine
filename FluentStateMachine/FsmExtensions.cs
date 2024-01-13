using FluentStateMachine._internal;
using System.Collections.Generic;

namespace FluentStateMachine;

public static partial class FsmExtensions
{
    public static object Trigger<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, TEvent e, object data = null)
        => fsm.TriggerAsync(e, data).Result;

    public static bool JumpTo<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, TState state, object data = null)
        => fsm.JumpToAsync(state, data).Result;

    public static void ResetTo<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, TState state)
        => fsm.ResetToAsync(state).Wait();

    public static void Reset<TState, TEvent>(this IStateMachine fsm)
        =>  fsm.ResetAsync().Wait();


#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
    public static IEnumerable<TState> GetStates<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, object data = null)
        => fsm.GetStatesAsync(data).ToList().Result;

    public static IEnumerable<TEvent> GetEvents<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, object data = null)
        => fsm.GetEventsAsync(data).ToList().Result;
#else
    public static IEnumerable<TState> GetStates<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, object data = null)
        => fsm.GetStatesAsync(data).Result;

    public static IEnumerable<TEvent> GetEvents<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, object data = null)
        => fsm.GetEventsAsync(data).Result;
#endif

}
