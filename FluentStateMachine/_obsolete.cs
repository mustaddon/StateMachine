using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FluentStateMachine;

public static partial class FsmExtensions
{
    [Obsolete("Method is deprecated, please use 'GetAvailableStates' instead.")]
    public static IEnumerable<TState> GetStates<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, object data = null)
        => fsm.GetAvailableStates(data);

    [Obsolete("Method is deprecated, please use 'GetAvailableEvents' instead.")]
    public static IEnumerable<TEvent> GetEvents<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, object data = null)
        => fsm.GetAvailableEvents(data);

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
    [Obsolete("Method is deprecated, please use 'GetAvailableStatesAsync' instead.")]
    public static async IAsyncEnumerable<TState> GetStatesAsync<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, object data = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach(var value in await fsm.GetAvailableStatesAsync(data, cancellationToken))
            yield return value;
    }

    [Obsolete("Method is deprecated, please use 'GetAvailableEventsAsync' instead.")]
    public static async IAsyncEnumerable<TEvent> GetEventsAsync<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, object data = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var value in await fsm.GetAvailableEventsAsync(data, cancellationToken))
            yield return value;
    }
#else
    [Obsolete("Method is deprecated, please use 'GetAvailableStatesAsync' instead.")]
    public static async Task<IEnumerable<TState>> GetStatesAsync<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, object data = null, CancellationToken cancellationToken = default)
        => await fsm.GetAvailableStatesAsync(data, cancellationToken);

    [Obsolete("Method is deprecated, please use 'GetAvailableEventsAsync' instead.")]
    public static async Task<IEnumerable<TEvent>> GetEventsAsync<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, object data = null, CancellationToken cancellationToken = default)
        => await fsm.GetAvailableEventsAsync(data, cancellationToken);
#endif
}