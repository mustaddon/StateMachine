using System;
using System.Collections.Generic;
using System.Text;

namespace RandomSolutions
{
    public interface IStateMachine<TState, TEvent> : IStateMachine
    {
        TState Current { get; }

        IEnumerable<TState> GetStates(params object[] data);

        IEnumerable<TEvent> GetEvents(params object[] data);

        object Trigger(TEvent e, params object[] data);

        bool JumpTo(TState state, params object[] data);
        
        void ResetTo(TState state);
    }

    public interface IStateMachine
    {
        void Reset();
    }
}
