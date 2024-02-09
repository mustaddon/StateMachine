﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FluentStateMachine._internal;

internal class FsmModel<TState, TEvent>
{
    public TState Start { get; set; }
    public Func<FsmTriggerArgs<TState, TEvent>, Task> OnTrigger { get; set; }
    public Func<FsmTriggerArgs<TState, TEvent>, Task> OnFire { get; set; }
    public Func<FsmCompleteArgs<TState, TEvent>, Task> OnComplete { get; set; }
    public Func<FsmExitArgs<TState, TEvent>, Task> OnExit { get; set; }
    public Func<FsmEnterArgs<TState, TEvent>, Task> OnEnter { get; set; }
    public Func<FsmEnterArgs<TState, TEvent>, Task> OnJump { get; set; }
    public Func<FsmResetArgs<TState, TEvent>, Task> OnReset { get; set; }
    public Func<FsmErrorArgs<TState, TEvent>, Task> OnError { get; set; }
    public Dictionary<TState, FsmStateModel<TState, TEvent>> States { get; set; } = [];
    public Dictionary<TEvent, FsmEventModel<TState, TEvent>> Events { get; set; } = [];
}

internal class FsmStateModel<TState, TEvent>
{
    public Func<FsmEnterArgs<TState, TEvent>, Task<bool>> Enable { get; set; }
    public Func<FsmEnterArgs<TState, TEvent>, Task> OnEnter { get; set; }
    public Func<FsmExitArgs<TState, TEvent>, Task> OnExit { get; set; }
    public Dictionary<TEvent, FsmEventModel<TState, TEvent>> Events { get; set; } = [];
}

internal class FsmEventModel<TState, TEvent>
{
    public Func<FsmTriggerArgs<TState, TEvent>, Task<bool>> Enable { get; set; }
    public Func<FsmTriggerArgs<TState, TEvent>, Task> Execute { get; set; }
    public Func<FsmTriggerArgs<TState, TEvent>, Task<TState>> JumpTo { get; set; }
}
