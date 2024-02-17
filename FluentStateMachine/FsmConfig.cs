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

    public FsmEventConfig<TState, TEvent, object, object> On(TEvent e) => On<object, object>(e);
    public FsmEventConfig<TState, TEvent, TData, object> On<TData>(TEvent e) => On<TData, object>(e);
    public FsmEventConfig<TState, TEvent, TData, TResult> On<TData, TResult>(TEvent e)
    {
        if (!Model.Events.TryGetValue(e, out var eventModel))
            Model.Events.Add(e, eventModel = new());

        return new FsmEventConfig<TState, TEvent, TData, TResult>(Root, eventModel, this);
    }

    public FsmEventConfig<TState, TEvent, TData, object> On<TData>() where TData : IFsmEvent<TData, object>
        => On<TData, object>((TEvent)(object)typeof(TData));
    public FsmEventConfig<TState, TEvent, TData, TResult> On<TData, TResult>() where TData : IFsmEvent<TData, TResult>
        => On<TData, TResult>((TEvent)(object)typeof(TData));

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

public class FsmEventConfig<TState, TEvent, TData, TResult> : FsmConfig<TState, TEvent>
{
    internal FsmEventConfig(FsmBuilder<TState, TEvent> root, FsmEventModel<TState, TEvent> model, FsmStateConfig<TState, TEvent> parent = null)
        : base(root)
    {
        Model = model;
        Parent = parent;
    }

    internal readonly FsmEventModel<TState, TEvent> Model;

    internal readonly FsmStateConfig<TState, TEvent> Parent;

    public FsmEventConfig<TState, TEvent, object, object> On(TEvent e) => On<object, object>(e);
    public FsmEventConfig<TState, TEvent, TArgsData, object> On<TArgsData>(TEvent e) => On<TArgsData, object>(e);
    public FsmEventConfig<TState, TEvent, TArgsData, TExecuteResult> On<TArgsData, TExecuteResult>(TEvent e) 
        => Parent != null ? Parent.On<TArgsData, TExecuteResult>(e) : Root.On<TArgsData, TExecuteResult>(e);

    public FsmEventConfig<TState, TEvent, TArgsData, object> On<TArgsData>() where TArgsData : IFsmEvent<TArgsData, object>
        => On<TArgsData, object>((TEvent)(object)typeof(TArgsData));
    public FsmEventConfig<TState, TEvent, TArgsData, TExecuteResult> On<TArgsData, TExecuteResult>() where TArgsData : IFsmEvent<TArgsData, TExecuteResult>
        => On<TArgsData, TExecuteResult>((TEvent)(object)typeof(TArgsData));

    internal FsmEventConfig<TState, TEvent, TData, TResult> Execute(Func<IFsmTriggerArgs<TState, TEvent, TData>, Task> fn)
    {
        Model.Execute = Cast(fn);
        return this;
    }

    public FsmEventConfig<TState, TEvent, TData, TResult> Execute(Func<IFsmTriggerArgs<TState, TEvent, TData>, Task<TResult>> fn)
    {
        Model.Execute = Cast(fn);
        return this;
    }

    public FsmEventConfig<TState, TEvent, TData, TResult> Enable(Func<IFsmTriggerArgs<TState, TEvent, TData>, Task<bool>> fn)
    {

        Model.Enable = Cast(fn);
        return this;
    }

    public FsmEventConfig<TState, TEvent, TData, TResult> JumpTo(Func<IFsmTriggerArgs<TState, TEvent, TData>, Task<TState>> fn)
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
