using System.Threading;
using System.Threading.Tasks;
using FluentStateMachine._internal;

namespace FluentStateMachine;

public partial class StateMachine<TState, TEvent>(FsmModel<TState, TEvent> model) : IStateMachine<TState, TEvent>
{
    private readonly FsmModel<TState, TEvent> _model = model;
    private readonly object _locker = new();

    public TState Current { get; private set; } = model.Start;


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
