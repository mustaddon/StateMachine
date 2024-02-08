using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FluentStateMachine;

public static class IStateMachineExtensions
{
    public static object Trigger<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, TEvent e, object data = null)
        => fsm.TriggerAsync(e, data).Result;

    public static bool JumpTo<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, TState state, object data = null)
        => fsm.JumpToAsync(state, data).Result;

    public static void ResetTo<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, TState state)
        => fsm.ResetToAsync(state).Wait();

    public static void Reset<TState, TEvent>(this IStateMachine fsm)
    => fsm.ResetAsync().Wait();

    public static bool IsAvailableState<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, TState value, object data = null)
        => fsm.IsAvailableStateAsync(value, data).Result;

    public static bool IsAvailableEvent<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, TEvent value, object data = null)
        => fsm.IsAvailableEventAsync(value, data).Result;

    public static ICollection<TState> GetAvailableStates<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, object data = null)
        => fsm.GetAvailableStatesAsync(data).Result;

    public static ICollection<TEvent> GetAvailableEvents<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, object data = null)
        => fsm.GetAvailableEventsAsync(data).Result;

    public static async Task<ICollection<TState>> GetAvailableStatesAsync<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, object data = null, CancellationToken cancellationToken = default)
    {
        var result = new HashSet<TState>();
        foreach (var value in fsm.States)
            if (await fsm.IsAvailableStateAsync(value, data, cancellationToken))
                result.Add(value);
        return result;
    }

    public static async Task<ICollection<TEvent>> GetAvailableEventsAsync<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, object data = null, CancellationToken cancellationToken = default)
    {
        var result = new HashSet<TEvent>();
        foreach (var value in fsm.Events)
            if (await fsm.IsAvailableEventAsync(value, data, cancellationToken))
                result.Add(value);
        return result;
    }
}
