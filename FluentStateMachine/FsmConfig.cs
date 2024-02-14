using System;
using System.Threading.Tasks;
using FluentStateMachine._internal;

namespace FluentStateMachine;

public class FsmConfig<TState, TEvent>
{
    internal FsmConfig(FsmBuilder<TState, TEvent> root)
    {
        Root = root;
    }

    internal readonly FsmBuilder<TState, TEvent> Root;

    public FsmStateConfig<TState, TEvent> State(TState state)
        => Root.State(state);

    public IStateMachine<TState, TEvent> Build(bool concurrent = false)
        => Root.Build(concurrent);
}

public class FsmStateConfig<TState, TEvent> : FsmConfig<TState, TEvent>
{
    internal FsmStateConfig(FsmBuilder<TState, TEvent> root, FsmStateModel<TState, TEvent> model)
        : base(root)
    {
        Model = model;
    }

    internal readonly FsmStateModel<TState, TEvent> Model;

    public FsmEventConfig<TState, TEvent> On(TEvent e)
    {
        if (!Model.Events.TryGetValue(e, out var eventModel))
            Model.Events.Add(e, eventModel = new());

        return new FsmEventConfig<TState, TEvent>(Root, eventModel, this);
    }

    public FsmStateConfig<TState, TEvent> OnEnter(Func<IFsmEnterArgs<TState, TEvent>, Task> action)
    {
        Model.OnEnter = action;
        return this;
    }

    public FsmStateConfig<TState, TEvent> OnExit(Func<IFsmExitArgs<TState, TEvent>, Task> action)
    {
        Model.OnExit = action;
        return this;
    }

    public FsmStateConfig<TState, TEvent> Enable(Func<IFsmEnterArgs<TState, TEvent>, Task<bool>> fn)
    {
        Model.Enable = fn;
        return this;
    }
}

public class FsmEventConfig<TState, TEvent> : FsmConfig<TState, TEvent>
{
    internal FsmEventConfig(FsmBuilder<TState, TEvent> root, FsmEventModel<TState, TEvent> model, FsmStateConfig<TState, TEvent> parent = null)
        : base(root)
    {
        Model = model;
        Parent = parent;
    }

    internal readonly FsmEventModel<TState, TEvent> Model;

    internal readonly FsmStateConfig<TState, TEvent> Parent;

    public FsmEventConfig<TState, TEvent> On(TEvent e) => Parent?.On(e) ?? Root.On(e);

    public FsmEventConfig<TState, TEvent> Execute(Func<IFsmTriggerArgs<TState, TEvent>, Task> fn)
    {
        Model.Execute = fn;
        return this;
    }

    public FsmEventConfig<TState, TEvent> Execute<TResult>(Func<IFsmTriggerArgs<TState, TEvent>, Task<TResult>> fn)
    {
        Model.Execute = fn;
        return this;
    }

    public FsmEventConfig<TState, TEvent> Enable(Func<IFsmTriggerArgs<TState, TEvent>, Task<bool>> fn)
    {
        Model.Enable = fn;
        return this;
    }

    public FsmEventConfig<TState, TEvent> JumpTo(Func<IFsmTriggerArgs<TState, TEvent>, Task<TState>> fn)
    {
        Model.JumpTo = fn;
        return this;
    }
}
