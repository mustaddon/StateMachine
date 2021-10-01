using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FluentStateMachine
{
    public interface IStateMachine<TState, TEvent> : IStateMachine
    {
        TState Current { get; }

        Task<object> TriggerAsync(TEvent e, object data = null, CancellationToken cancellationToken = default);
        Task<bool> JumpToAsync(TState state, object data = null, CancellationToken cancellationToken = default);
        Task ResetToAsync(TState state, CancellationToken cancellationToken = default);

#if NETSTANDARD2_1_OR_GREATER
        IAsyncEnumerable<TState> GetStatesAsync(object data = null, CancellationToken cancellationToken = default);
        IAsyncEnumerable<TEvent> GetEventsAsync(object data = null, CancellationToken cancellationToken = default);
#else
        Task<IEnumerable<TState>> GetStatesAsync(object data = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEvent>> GetEventsAsync(object data = null, CancellationToken cancellationToken = default);
#endif

    }

    public interface IStateMachine
    {
        Task ResetAsync(CancellationToken cancellationToken = default);
    }
}
