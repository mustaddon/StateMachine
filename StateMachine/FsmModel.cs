using System;
using System.Collections.Generic;
using System.Text;

namespace RandomSolutions
{
    public class FsmModel<TState, TEvent>
    {
        public TState Start { get; set; }
        public Action<FsmEnterArgs<TState, TEvent>> OnChange { get; set; }
        public Action<FsmTriggerArgs<TState, TEvent>> OnTrigger { get; set; }
        public Action<FsmTriggerArgs<TState, TEvent>> OnFire { get; set; }
        public Action<FsmException> OnError { get; set; }
        public Dictionary<TState, FsmStateModel<TState, TEvent>> States { get; set; } = new Dictionary<TState, FsmStateModel<TState, TEvent>>();
    }

    public class FsmStateModel<TState, TEvent>
    {
        public Dictionary<TEvent, FsmEventModel<TState, TEvent>> Events { get; set; } = new Dictionary<TEvent, FsmEventModel<TState, TEvent>>();
        public Func<FsmEnterArgs<TState, TEvent>, bool> Enable { get; set; }
        public Action<FsmEnterArgs<TState, TEvent>> OnEnter { get; set; }
        public Action<FsmExitArgs<TState, TEvent>> OnExit { get; set; }
    }

    public class FsmEventModel<TState, TEvent>
    {
        public Func<FsmTriggerArgs<TState, TEvent>, bool> Enable { get; set; }
        public Func<FsmTriggerArgs<TState, TEvent>, object> Execute { get; set; }
        public Func<FsmTriggerArgs<TState, TEvent>, TState> JumpTo { get; set; }
    }
}
