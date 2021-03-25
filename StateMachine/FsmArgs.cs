using System;
using System.Collections.Generic;
using System.Text;

namespace RandomSolutions
{
    public class FsmArgs<TState, TEvent>
    {
        public IStateMachine<TState, TEvent> Fsm { get; internal set; }
    }

    public class FsmResetArgs<TState, TEvent> : FsmArgs<TState, TEvent>
    {
        public TState PrevState { get; internal set; }
    }

    public class FsmDataArgs<TState, TEvent> : FsmArgs<TState, TEvent>
    {
        public object[] Data { get; internal set; }
    }

    public class FsmExitArgs<TState, TEvent> : FsmDataArgs<TState, TEvent>
    {
        public TState NextState { get; internal set; }
    }

    public class FsmEnterArgs<TState, TEvent> : FsmDataArgs<TState, TEvent>
    {
        public TState PrevState { get; internal set; }
    }

    public class FsmErrorArgs<TState, TEvent> : FsmDataArgs<TState, TEvent>
    {
        public string Message { get; internal set; }
    }

    public class FsmTriggerArgs<TState, TEvent> : FsmDataArgs<TState, TEvent>
    {
        public TEvent Event { get; internal set; }
    }

    public class FsmCompleteArgs<TState, TEvent> : FsmTriggerArgs<TState, TEvent>
    {
        public object Result { get; internal set; }
    }
}
