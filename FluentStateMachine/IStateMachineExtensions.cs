using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FluentStateMachine;

public static class IStateMachineExtensions
{
    public static Task<object> TriggerAsync<TEvent>(this IEventController<TEvent> fsm, TEvent e, object data = default, CancellationToken cancellationToken = default)
        => fsm.TriggerAsync<object>(e, data, cancellationToken);

    public static TResult Trigger<TEvent, TResult>(this IEventController<TEvent> fsm, TEvent e, object data = null)
        => fsm.TriggerAsync<TResult>(e, data).Result;

    public static object Trigger<TEvent>(this IEventController<TEvent> fsm, TEvent e, object data = null)
        => fsm.TriggerAsync<object>(e, data).Result;

    public static bool JumpTo<TState>(this IStateController<TState> fsm, TState state, object data = null)
        => fsm.JumpToAsync(state, data).Result;

    public static void ResetTo<TState>(this IStateController<TState> fsm, TState state)
        => fsm.ResetToAsync(state).Wait();

    public static void Reset(this IStateMachine fsm)
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



    public static Task<TResult> TriggerAsyncX<TEvent, TData, TResult>(this IEventController<TEvent> fsm, IFsmEvent<TData, TResult> e, TData data = default, CancellationToken cancellationToken = default)
        where TEvent : IFsmEvent
        => fsm.TriggerAsync<TResult>((TEvent)e, data, cancellationToken);

    public static TResult TriggerX<TEvent, TData, TResult>(this IEventController<TEvent> fsm, IFsmEvent<TData, TResult> e, TData data = default)
        where TEvent : IFsmEvent
        => fsm.TriggerAsync<TResult>((TEvent)e, data).Result;

    public static Task<object> TriggerAsyncX<TEvent>(this IEventController<TEvent> fsm, IFsmEvent e, object data = null, CancellationToken cancellationToken = default)
        where TEvent : IFsmEvent
        => fsm.TriggerAsync<object>((TEvent)e, data, cancellationToken);

    public static object TriggerX<TEvent>(this IEventController<TEvent> fsm, IFsmEvent e, object data = null, CancellationToken cancellationToken = default)
        where TEvent : IFsmEvent
        => fsm.TriggerAsync<object>((TEvent)e, data, cancellationToken).Result;
}
