using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FluentStateMachine
{
    public static class FsmExtensions
    {

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        private static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> items, CancellationToken cancellationToken = default)
        {
            var result = new List<T>();

            await foreach (var item in items.WithCancellation(cancellationToken).ConfigureAwait(false))
                result.Add(item);

            return result;
        }

        public static IEnumerable<TState> GetStates<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, object data = null)
        {
            return fsm.GetStatesAsync(data).ToListAsync().Result;
        }

        public static IEnumerable<TEvent> GetEvents<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, object data = null)
        {
            return fsm.GetEventsAsync(data).ToListAsync().Result;
        }
#else
        public static IEnumerable<TState> GetStates<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, object data = null)
        {
            return fsm.GetStatesAsync(data).Result;
        }

        public static IEnumerable<TEvent> GetEvents<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, object data = null)
        {
            return fsm.GetEventsAsync(data).Result;
        }
#endif


        public static object Trigger<TState, TEvent>(this IStateMachine<TState, TEvent> fsm,
            TEvent e, object data = null)
        {
            return fsm.TriggerAsync(e, data).Result;
        }

        public static bool JumpTo<TState, TEvent>(this IStateMachine<TState, TEvent> fsm,
            TState state, object data = null)
        {
            return fsm.JumpToAsync(state, data).Result;
        }

        public static void ResetTo<TState, TEvent>(this IStateMachine<TState, TEvent> fsm, TState state)
        {
            fsm.ResetToAsync(state).Wait();
        }

        public static void Reset<TState, TEvent>(this IStateMachine fsm)
        {
            fsm.ResetAsync().Wait();
        }
    }
}
