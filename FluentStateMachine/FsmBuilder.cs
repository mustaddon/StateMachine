using FluentStateMachine._internal;
using System;
using System.Threading.Tasks;

namespace FluentStateMachine;

public class FsmBuilder<TState, TEvent>
    where TState : notnull
    where TEvent : notnull
{
    public FsmBuilder(TState start)
    {
        _model = new() { Start = start };
        _model.States.Add(start, new());
    }

    private readonly FsmModel<TState, TEvent> _model;

    public FsmBuilder<TState, TEvent> OnReset(Func<IFsmEnterArgs<TState, TEvent>, Task> action)
    {
        _model.OnReset = action;
        return this;
    }

    public FsmBuilder<TState, TEvent> OnExit(Func<IFsmExitArgs<TState, TEvent>, Task> action)
    {
        _model.OnExit = action;
        return this;
    }

    public FsmBuilder<TState, TEvent> OnEnter(Func<IFsmEnterArgs<TState, TEvent>, Task> action)
    {
        _model.OnEnter = action;
        return this;
    }

    public FsmBuilder<TState, TEvent> OnJump(Func<IFsmEnterArgs<TState, TEvent>, Task> action)
    {
        _model.OnJump = action;
        return this;
    }

    public FsmBuilder<TState, TEvent> OnTrigger(Func<IFsmTriggerArgs<TState, TEvent>, Task> action)
    {
        _model.OnTrigger = action;
        return this;
    }

    public FsmBuilder<TState, TEvent> OnFire(Func<IFsmTriggerArgs<TState, TEvent>, Task> action)
    {
        _model.OnFire = action;
        return this;
    }

    public FsmBuilder<TState, TEvent> OnComplete(Func<IFsmCompleteArgs<TState, TEvent>, Task> action)
    {
        _model.OnComplete = action;
        return this;
    }

    public FsmBuilder<TState, TEvent> OnError(Func<IFsmErrorArgs<TState, TEvent>, Task> action)
    {
        _model.OnError = action;
        return this;
    }

    public FsmEventConfig<TState, TEvent, TData, TResult> On<TData, TResult>(TEvent e)
    {
        if (!_model.Events.TryGetValue(e, out var eventModel))
            _model.Events.Add(e, eventModel = new());

        return new FsmEventConfig<TState, TEvent, TData, TResult>(this, eventModel);
    }

    public FsmStateConfig<TState, TEvent> State(TState state)
    {
        if (!_model.States.TryGetValue(state, out var stateModel))
            _model.States.Add(state, stateModel = new());

        return new FsmStateConfig<TState, TEvent>(this, stateModel);
    }

    public IStateMachine<TState, TEvent> Build(bool concurrent = false)
    {
        var fsm = new StateMachine<TState, TEvent>(_model);
        return !concurrent ? fsm : new FsmConcurrentDecorator<TState, TEvent>(fsm);
    }
}
