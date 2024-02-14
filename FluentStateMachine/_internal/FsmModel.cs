using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentStateMachine._internal;

internal class FsmModel<TState, TEvent>
{
    public TState Start { get; set; }
    public Func<IFsmTriggerArgs<TState, TEvent>, Task> OnTrigger { get; set; }
    public Func<IFsmTriggerArgs<TState, TEvent>, Task> OnFire { get; set; }
    public Func<IFsmCompleteArgs<TState, TEvent>, Task> OnComplete { get; set; }
    public Func<IFsmExitArgs<TState, TEvent>, Task> OnExit { get; set; }
    public Func<IFsmEnterArgs<TState, TEvent>, Task> OnEnter { get; set; }
    public Func<IFsmEnterArgs<TState, TEvent>, Task> OnJump { get; set; }
    public Func<IFsmEnterArgs<TState, TEvent>, Task> OnReset { get; set; }
    public Func<IFsmErrorArgs<TState, TEvent>, Task> OnError { get; set; }
    public Dictionary<TState, FsmStateModel<TState, TEvent>> States { get; set; } = [];
    public Dictionary<TEvent, FsmEventModel<TState, TEvent>> Events { get; set; } = [];
}

internal class FsmStateModel<TState, TEvent>
{
    public Func<IFsmEnterArgs<TState, TEvent>, Task<bool>> Enable { get; set; }
    public Func<IFsmEnterArgs<TState, TEvent>, Task> OnEnter { get; set; }
    public Func<IFsmExitArgs<TState, TEvent>, Task> OnExit { get; set; }
    public Dictionary<TEvent, FsmEventModel<TState, TEvent>> Events { get; set; } = [];
}

internal class FsmEventModel<TState, TEvent>
{
    public Func<IFsmTriggerArgs<TState, TEvent>, Task<bool>> Enable { get; set; }
    public Func<IFsmTriggerArgs<TState, TEvent>, Task> Execute { get; set; }
    public Func<IFsmTriggerArgs<TState, TEvent>, Task<TState>> JumpTo { get; set; }
}
