using System;
using System.Collections.Generic;
using System.Text;

namespace RandomSolutions
{
    public class FsmArgs<TState, TEvent>
    {
        public IStateMachine<TState, TEvent> Fsm { get; internal set; }
        public object[] Data { get; internal set; }
    }

    public class FsmTriggerArgs<TState, TEvent> : FsmArgs<TState, TEvent>
    {
        public TEvent Event { get; internal set; }
    }

    public class FsmExitArgs<TState, TEvent> : FsmArgs<TState, TEvent>
    {
        public TState NextState { get; internal set; }
    }

    public class FsmEnterArgs<TState, TEvent> : FsmArgs<TState, TEvent>
    {
        public TState PrevState { get; internal set; }
    }
}
