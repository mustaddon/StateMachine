using FluentStateMachine._internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FluentStateMachine;

public sealed class StateMachine<TState, TEvent> : IStateMachine<TState, TEvent>
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

    public ICollection<TEvent> Events { 
        get {
            var state = Current;

            if (_events == null || !EqualityComparer<TState>.Default.Equals(state, _eventsState)) {
                _events = new(() => new(_model.Events.Keys.Concat(_model.States[state].Events.Keys)));
                _eventsState = state;
            }

            return _events.Value;
        }
    }

    private Lazy<HashSet<TEvent>> _events;
    private TState _eventsState;


    public Task<bool> IsAvailableStateAsync(TState value, object data = null, CancellationToken cancellationToken = default)
    {
        if (!_model.States.TryGetValue(value, out var stateModel))
            return Task.FromResult(false);

        if (stateModel.Enable == null)
            return Task.FromResult(true);

        return stateModel.Enable?.Invoke(new FsmEnterArgs<TState, TEvent>
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
        var args = new FsmTriggerArgs<TState, TEvent>
        {
            Fsm = this,
            Event = e,
            Data = data,
            CancellationToken = cancellationToken,
        };

        await OnTrigger(args).ConfigureAwait(false);

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

        await OnFire(args).ConfigureAwait(false);

        var result = eventModel.Execute == null ? null
            : await eventModel.Execute(args).ConfigureAwait(false);

        if (eventModel.JumpTo != null)
        {
            var next = await eventModel.JumpTo(args).ConfigureAwait(false);
            var done = await JumpToAsync(next, data, default).ConfigureAwait(false);

            if (eventModel.Execute == null)
                result = done;
        }

        await OnComplete(args, result).ConfigureAwait(false);

        return result;
    }

    public async Task<bool> JumpToAsync(TState next, object data = null, CancellationToken cancellationToken = default)
    {
        var enterArgs = new FsmEnterArgs<TState, TEvent>
        {
            Fsm = this,
            PrevState = Current,
            Data = data,
            CancellationToken = cancellationToken,
        };

        if (!_model.States.ContainsKey(next))
        {
            await OnError(enterArgs, "Next state '{0}' not found", next).ConfigureAwait(false);
            return false;
        }

        var nextModel = _model.States[next];

        if (nextModel.Enable != null && !await nextModel.Enable(enterArgs).ConfigureAwait(false))
        {
            await OnError(enterArgs, "Next state '{0}' disabled", next).ConfigureAwait(false);
            return false;
        }

        cancellationToken.ThrowIfCancellationRequested();

        await OnExit(enterArgs, next).ConfigureAwait(false);

        lock (_locker)
            Current = next;

        await OnEnter(enterArgs).ConfigureAwait(false);

        await OnJump(enterArgs).ConfigureAwait(false);

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

        await OnReset(args).ConfigureAwait(false);
    }



    private Task OnReset(FsmResetArgs<TState, TEvent> args)
    {
        return _model.OnReset?.Invoke(args) ?? FrameworkExt.CompletedTask;
    }

    private Task OnTrigger(FsmTriggerArgs<TState, TEvent> args)
    {
        return _model.OnTrigger?.Invoke(args) ?? FrameworkExt.CompletedTask;
    }

    private Task OnFire(FsmTriggerArgs<TState, TEvent> args)
    {
        return _model.OnFire?.Invoke(args) ?? FrameworkExt.CompletedTask;
    }

    private async Task OnExit(FsmDataArgs<TState, TEvent> args, TState next)
    {
        var exitArgs = new FsmExitArgs<TState, TEvent>
        {
            Fsm = this,
            Data = args.Data,
            CancellationToken = args.CancellationToken,
            NextState = next,
        };

        if (_model.OnExit != null)
            await _model.OnExit(exitArgs).ConfigureAwait(false);

        if (_model.States[Current].OnExit != null)
            await _model.States[Current].OnExit(exitArgs).ConfigureAwait(false);
    }

    private async Task OnEnter(FsmEnterArgs<TState, TEvent> args)
    {
        if (_model.OnEnter != null)
            await _model.OnEnter(args).ConfigureAwait(false);

        if (_model.States[Current].OnEnter != null)
            await _model.States[Current].OnEnter(args).ConfigureAwait(false);
    }

    private Task OnJump(FsmEnterArgs<TState, TEvent> args)
    {
        return _model.OnJump?.Invoke(args) ?? FrameworkExt.CompletedTask;
    }

    private Task OnComplete(FsmTriggerArgs<TState, TEvent> args, object result)
    {
        return _model.OnComplete?.Invoke(new FsmCompleteArgs<TState, TEvent>
        {
            Fsm = this,
            Event = args.Event,
            Data = args.Data,
            CancellationToken = args.CancellationToken,
            Result = result,
        }) ?? FrameworkExt.CompletedTask;
    }

    private Task OnError(FsmDataArgs<TState, TEvent> args, string message, params object[] formatArgs)
    {
        return _model.OnError?.Invoke(new FsmErrorArgs<TState, TEvent>
        {
            Fsm = this,
            Data = args.Data,
            CancellationToken = args.CancellationToken,
            Message = string.Format(message, formatArgs),
        }) ?? FrameworkExt.CompletedTask;
    }



}
