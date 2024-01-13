using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FluentStateMachine;

public partial class StateMachine<TState, TEvent>
{
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
    public async IAsyncEnumerable<TState> GetStatesAsync(object data = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var args = new FsmEnterArgs<TState, TEvent>
        {
            Fsm = this,
            PrevState = Current,
            Data = data,
            CancellationToken = cancellationToken,
        };

        foreach (var kvp in _model.States)
            if (kvp.Value.Enable == null || await kvp.Value.Enable(args).ConfigureAwait(false))
                yield return kvp.Key;
    }
#else
    public async Task<IEnumerable<TState>> GetStatesAsync(object data = null, CancellationToken cancellationToken = default)
    {
        var args = new FsmEnterArgs<TState, TEvent>
        {
            Fsm = this,
            PrevState = Current,
            Data = data,
            CancellationToken = cancellationToken,
        };

        var stateTasks = _model.States
            .Select(x => new { State = x.Key, Task = x.Value.Enable?.Invoke(args) })
            .ToList();

        await Task.WhenAll(stateTasks.Select(x => x.Task).Where(x => x != null)).ConfigureAwait(false);

        return stateTasks
            .Where(x => x.Task?.Result != false)
            .Select(x => x.State);
    }
#endif
}
