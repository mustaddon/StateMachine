using System;
using System.Threading.Tasks;
using FluentStateMachine._internal;

namespace FluentStateMachine;

public class FsmBuilder<TState, TEvent>(TState start)
{
    private readonly FsmModel<TState, TEvent> _model = new() { Start = start };

    public FsmBuilder<TState, TEvent> OnReset(Func<FsmResetArgs<TState, TEvent>, Task> action)
    {
        _model.OnReset = action;
        return this;
    }

    public FsmBuilder<TState, TEvent> OnExit(Func<FsmExitArgs<TState, TEvent>, Task> action)
    {
        _model.OnExit = action;
        return this;
    }

    public FsmBuilder<TState, TEvent> OnEnter(Func<FsmEnterArgs<TState, TEvent>, Task> action)
    {
        _model.OnEnter = action;
        return this;
    }

    public FsmBuilder<TState, TEvent> OnJump(Func<FsmEnterArgs<TState, TEvent>, Task> action)
    {
        _model.OnJump = action;
        return this;
    }

    public FsmBuilder<TState, TEvent> OnTrigger(Func<FsmTriggerArgs<TState, TEvent>, Task> action)
    {
        _model.OnTrigger = action;
        return this;
    }

    public FsmBuilder<TState, TEvent> OnFire(Func<FsmTriggerArgs<TState, TEvent>, Task> action)
    {
        _model.OnFire = action;
        return this;
    }

    public FsmBuilder<TState, TEvent> OnComplete(Func<FsmCompleteArgs<TState, TEvent>, Task> action)
    {
        _model.OnComplete = action;
        return this;
    }

    public FsmBuilder<TState, TEvent> OnError(Func<FsmErrorArgs<TState, TEvent>, Task> action)
    {
        _model.OnError = action;
        return this;
    }

    public FsmEventConfig<TState, TEvent> On(TEvent e)
    {
        var eventModel = new FsmEventModel<TState, TEvent>();

        if (_model.Events.ContainsKey(e))
            _model.Events[e] = eventModel;
        else
            _model.Events.Add(e, eventModel);

        return new FsmEventConfig<TState, TEvent>(this, eventModel);
    }

    public FsmStateConfig<TState, TEvent> State(TState state)
    {
        var stateModel = new FsmStateModel<TState, TEvent>();

        if (_model.States.ContainsKey(state))
            _model.States[state] = stateModel;
        else
            _model.States.Add(state, stateModel);

        return new FsmStateConfig<TState, TEvent>(this, stateModel);
    }

    public IStateMachine<TState, TEvent> Build()
    {
        if (!_model.States.ContainsKey(_model.Start))
            throw new Exception($"States collection is not contains start point '{_model.Start}'");

        return new StateMachine<TState, TEvent>(_model);
    }
}
