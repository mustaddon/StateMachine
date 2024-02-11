using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FluentStateMachine._internal;

internal sealed class StateMachine<TState, TEvent> : IStateMachine<TState, TEvent>
{
    internal StateMachine(FsmModel<TState, TEvent> model)
    {
        _model = model;
        Current = model.Start;
    }

    private readonly FsmModel<TState, TEvent> _model;
    private readonly object _locker = new();

    public TState Current { get; private set; }

    public ICollection<TState> States => _model.States.Keys;

    public ICollection<TEvent> Events => _model.Events.Count == 0 ? _model.States[Current].Events.Keys
        : _model.States[Current].Events.Count == 0 ? _model.Events.Keys
        : (_events ??= []).GetOrAdd(Current, k => new ReadOnlyHashSet<TEvent>(_model.Events.Keys.Concat(_model.States[k].Events.Keys)));

    private ConcurrentDictionary<TState, ICollection<TEvent>> _events;


    public Task<bool> IsAvailableStateAsync(TState value, object data = null, CancellationToken cancellationToken = default)
    {
        if (!_model.States.TryGetValue(value, out var stateModel))
            return Task.FromResult(false);

        if (stateModel.Enable == null)
            return Task.FromResult(true);

        return stateModel.Enable(new FsmEnterExitArgs<TState, TEvent>
        {
            Fsm = this,
            PrevState = Current,
            Data = data,
            CancellationToken = cancellationToken,
        });
    }

    public Task<bool> IsAvailableEventAsync(TEvent value, object data = null, CancellationToken cancellationToken = default)
    {
        if (!_model.States[Current].Events.TryGetValue(value, out var eventModel)
            && !_model.Events.TryGetValue(value, out eventModel))
            return Task.FromResult(false);

        if (eventModel.Enable == null)
            return Task.FromResult(true);

        return eventModel.Enable(new FsmTriggerArgs<TState, TEvent>
        {
            Fsm = this,
            Data = data,
            CancellationToken = cancellationToken,
        });
    }


    public async Task<object> TriggerAsync(TEvent e, object data = null, CancellationToken cancellationToken = default)
    {
        var args = new FsmCompleteArgs<TState, TEvent>
        {
            Fsm = this,
            Event = e,
            Data = data,
            CancellationToken = cancellationToken,
            PrevState = Current,
        };

        if (_model.OnTrigger != null)
            await _model.OnTrigger(args).ConfigureAwait(false);

        var stateModel = _model.States[Current];

        if (!stateModel.Events.TryGetValue(e, out var eventModel) && !_model.Events.TryGetValue(e, out eventModel))
        {
            await OnError(args, "Event '{0}' not found", e).ConfigureAwait(false);
            return null;
        }

        if (eventModel.Enable != null && !await eventModel.Enable(args).ConfigureAwait(false))
        {
            await OnError(args, "Event '{0}' disabled", e).ConfigureAwait(false);
            return null;
        }

        cancellationToken.ThrowIfCancellationRequested();

        if (_model.OnFire != null)
            await _model.OnFire(args).ConfigureAwait(false);

        object result = null;

        if (eventModel.Execute != null)
        {
            var executeTask = eventModel.Execute(args);
            await executeTask.ConfigureAwait(false);

            if (eventModel.Execute.Method.ReturnType.IsGenericType)
                result = executeTask.GetResult();
        }

        if (eventModel.JumpTo != null)
        {
            var next = await eventModel.JumpTo(args).ConfigureAwait(false);
            var done = await JumpToAsync(next, data, default).ConfigureAwait(false);

            if (eventModel.Execute == null)
                result = done;
        }

        args.Result = result;

        if (_model.OnComplete != null)
            await _model.OnComplete(args).ConfigureAwait(false);

        return result;
    }

    public async Task<bool> JumpToAsync(TState next, object data = null, CancellationToken cancellationToken = default)
    {
        var args = new FsmEnterExitArgs<TState, TEvent>
        {
            Fsm = this,
            Data = data,
            CancellationToken = cancellationToken,
            PrevState = Current,
            NextState = next,
        };

        if (!_model.States.ContainsKey(next))
        {
            await OnError(args, "Next state '{0}' not found", next).ConfigureAwait(false);
            return false;
        }

        var nextModel = _model.States[next];

        if (nextModel.Enable != null && !await nextModel.Enable(args).ConfigureAwait(false))
        {
            await OnError(args, "Next state '{0}' disabled", next).ConfigureAwait(false);
            return false;
        }

        cancellationToken.ThrowIfCancellationRequested();

        if (_model.OnExit != null)
            await _model.OnExit(args).ConfigureAwait(false);

        if (_model.States[Current].OnExit != null)
            await _model.States[Current].OnExit(args).ConfigureAwait(false);

        lock (_locker)
            Current = next;

        if (_model.OnEnter != null)
            await _model.OnEnter(args).ConfigureAwait(false);

        if (_model.States[Current].OnEnter != null)
            await _model.States[Current].OnEnter(args).ConfigureAwait(false);

        if (_model.OnJump != null)
            await _model.OnJump(args).ConfigureAwait(false);

        return true;
    }

    public Task ResetAsync(CancellationToken cancellationToken = default) => ResetToAsync(_model.Start, cancellationToken);

    public async Task ResetToAsync(TState state, CancellationToken cancellationToken = default)
    {
        var args = new FsmResetArgs<TState, TEvent>
        {
            Fsm = this,
            PrevState = Current,
            CancellationToken = cancellationToken,
        };

        cancellationToken.ThrowIfCancellationRequested();

        lock (_locker)
            Current = state;

        if (_model.OnReset != null)
            await _model.OnReset(args).ConfigureAwait(false);
    }

    private Task OnError(FsmErrorArgs<TState, TEvent> args, string message, params object[] formatArgs)
    {
        if (_model.OnError == null)
            return Task.CompletedTask;

        args.Error = string.Format(message, formatArgs);
        return _model.OnError(args);
    }



}
