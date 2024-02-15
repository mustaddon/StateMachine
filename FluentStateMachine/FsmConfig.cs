using FluentStateMachine._internal;
using System;
using System.Threading.Tasks;

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

    public FsmEventConfig<TState, TEvent, TData> On<TData>(TEvent e)
    {
        if (!Model.Events.TryGetValue(e, out var eventModel))
            Model.Events.Add(e, eventModel = new());

        return new FsmEventConfig<TState, TEvent, TData>(Root, eventModel, this);
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

public class FsmEventConfig<TState, TEvent, TData> : FsmConfig<TState, TEvent>
{
    internal FsmEventConfig(FsmBuilder<TState, TEvent> root, FsmEventModel<TState, TEvent> model, FsmStateConfig<TState, TEvent> parent = null)
        : base(root)
    {
        Model = model;
        Parent = parent;
    }

    internal readonly FsmEventModel<TState, TEvent> Model;

    internal readonly FsmStateConfig<TState, TEvent> Parent;

    public FsmEventConfig<TState, TEvent, T> On<T>(TEvent e) => Parent != null ? Parent.On<T>(e) : Root.On<T>(e);

    public FsmEventConfig<TState, TEvent, TData> Execute(Func<IFsmTriggerArgs<TState, TEvent, TData>, Task> fn)
    {
        Model.Execute = Cast(fn);
        return this;
    }

    public FsmEventConfig<TState, TEvent, TData> Execute<TResult>(Func<IFsmTriggerArgs<TState, TEvent, TData>, Task<TResult>> fn)
    {
        Model.Execute = Cast(fn);
        return this;
    }

    public FsmEventConfig<TState, TEvent, TData> Enable(Func<IFsmTriggerArgs<TState, TEvent, TData>, Task<bool>> fn)
    {

        Model.Enable = Cast(fn);
        return this;
    }

    public FsmEventConfig<TState, TEvent, TData> JumpTo(Func<IFsmTriggerArgs<TState, TEvent, TData>, Task<TState>> fn)
    {
        Model.JumpTo = Cast(fn);
        return this;
    }

    static Func<IFsmTriggerArgs<TState, TEvent>, T> Cast<T>(Func<IFsmTriggerArgs<TState, TEvent, TData>, T> fn)
    {
        if (typeof(TData) == typeof(object))
            return x => fn(x as IFsmTriggerArgs<TState, TEvent, TData>);

        return x => fn(new FsmTriggerArgsCast<TState, TEvent, TData>(x));
    }
}
