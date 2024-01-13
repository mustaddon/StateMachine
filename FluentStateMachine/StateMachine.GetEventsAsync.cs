using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FluentStateMachine;

public partial class StateMachine<TState, TEvent>
{
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
    public async IAsyncEnumerable<TEvent> GetEventsAsync(object data = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var stateModel = _model.States[Current];

        var events = _model.Events
            .Where(x => !stateModel.Events.ContainsKey(x.Key))
            .Concat(stateModel.Events);

        var args = new FsmTriggerArgs<TState, TEvent>
        {
            Fsm = this,
            Data = data,
            CancellationToken = cancellationToken,
        };

        foreach (var kvp in events)
        {
            args.Event = kvp.Key;

            if (kvp.Value.Enable == null || await kvp.Value.Enable(args).ConfigureAwait(false))
                yield return kvp.Key;
        }
    }
#else
    public async Task<IEnumerable<TEvent>> GetEventsAsync(object data = null, CancellationToken cancellationToken = default)
    {
        var stateModel = _model.States[Current];

        var eventTasks = _model.Events
            .Where(x => !stateModel.Events.ContainsKey(x.Key))
            .Concat(stateModel.Events)
            .Select(x => new
            {
                Event = x.Key,
                Task = x.Value.Enable?.Invoke(new FsmTriggerArgs<TState, TEvent>
                {
                    Fsm = this,
                    Event = x.Key,
                    Data = data,
                    CancellationToken = cancellationToken,
                })
            })
            .ToList();

        await Task.WhenAll(eventTasks.Select(x => x.Task).Where(x => x != null)).ConfigureAwait(false);

        return eventTasks
            .Where(x => x.Task?.Result != false)
            .Select(x => x.Event);
    }
#endif
}
