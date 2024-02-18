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
    private bool _jump = false;

    public TState Current { get; private set; }
    public TEvent LastEvent { get; private set; }

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

        return stateModel.Enable(new FsmEnterArgs<TState, TEvent>(this)
        {
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

        return eventModel.Enable(new FsmTriggerArgs<TState, TEvent>(this, value)
        {
            Data = data,
            CancellationToken = cancellationToken,
        });
    }


    public async Task<TResult> TriggerAsync<TResult>(TEvent e, object data = null, CancellationToken cancellationToken = default)
    {
        var args = new FsmCompleteArgs<TState, TEvent>(this, e)
        {
            Data = data,
            CancellationToken = cancellationToken,
            PrevState = Current,
        };

        if (_model.OnTrigger != null)
            await _model.OnTrigger(args).ConfigureAwait(false);

        var state = Current;
        var stateModel = _model.States[state];

        if (!stateModel.Events.TryGetValue(e, out var eventModel) && !_model.Events.TryGetValue(e, out eventModel))
        {
            await OnError(args, "Event '{0}' not found (state '{1}')", e, state).ConfigureAwait(false);
            return default;
        }

        if (eventModel.Enable != null && !await eventModel.Enable(args).ConfigureAwait(false))
        {
            await OnError(args, "Event '{0}' disabled (state '{1}')", e, state).ConfigureAwait(false);
            return default;
        }

        cancellationToken.ThrowIfCancellationRequested();

        LastEvent = e;

        if (_model.OnFire != null)
            await _model.OnFire(args).ConfigureAwait(false);

        TResult result = default;

        if (eventModel.Execute != null)
        {
            var executeTask = eventModel.Execute(args);
            await executeTask.ConfigureAwait(false);

            if (eventModel.Execute.Method.ReturnType.IsGenericType || executeTask.IsGeneric())
                result = executeTask.GetResult<TResult>();
        }

        if (eventModel.JumpTo != null)
        {
            var next = await eventModel.JumpTo(args).ConfigureAwait(false);
            await JumpToAsync(next, data, default).ConfigureAwait(false);
        }

        if (_model.OnComplete != null)
        {
            args.Result = result;
            await _model.OnComplete(args).ConfigureAwait(false);
        }

        return result;
    }

    public async Task JumpToAsync(TState next, object data = null, CancellationToken cancellationToken = default)
    {
        var args = new FsmJumpArgs<TState, TEvent>(this)
        {
            Data = data,
            CancellationToken = cancellationToken,
            PrevState = Current,
            NextState = next,
        };

        if (!_model.States.TryGetValue(next, out var nextModel))
        {
            await OnError(args, "Next state '{0}' not found (state '{1}')", next, args.PrevState).ConfigureAwait(false);
            return;
        }

        if (nextModel.Enable != null && !await nextModel.Enable(args).ConfigureAwait(false))
        {
            await OnError(args, "Next state '{0}' disabled (state '{1}')", next, args.PrevState).ConfigureAwait(false);
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        _jump = true;

        if (_model.OnExit != null)
        {
            await _model.OnExit(args).ConfigureAwait(false);
            if (!_jump) return;
        }

        var currentModel = _model.States[Current];

        if (currentModel.OnExit != null)
        {
            await currentModel.OnExit(args).ConfigureAwait(false);
            if (!_jump) return;
        }

        Current = next;

        if (_model.OnEnter != null)
        {
            await _model.OnEnter(args).ConfigureAwait(false);
            if (!_jump) return;
        }

        if (nextModel.OnEnter != null)
        {
            await nextModel.OnEnter(args).ConfigureAwait(false);
            if (!_jump) return;
        }

        if (_model.OnJump != null)
            await _model.OnJump(args).ConfigureAwait(false);

        _jump = false;
    }

    public Task ResetAsync(CancellationToken cancellationToken = default) => ResetToAsync(_model.Start, cancellationToken);

    public async Task ResetToAsync(TState state, CancellationToken cancellationToken = default)
    {
        var prevState = Current;
        Current = state;
        _jump = false;

        if (_model.OnReset != null)
            await _model.OnReset(new FsmEnterArgs<TState, TEvent>(this)
            {
                PrevState = prevState,
                CancellationToken = cancellationToken,
            }).ConfigureAwait(false);

        LastEvent = default;
    }

    private Task OnError(FsmErrorArgs<TState, TEvent> args, string message, params object[] formatArgs)
    {
        if (_model.OnError == null)
            return Task.CompletedTask;

        args.Error = string.Format(message, formatArgs);
        return _model.OnError(args);
    }



}
