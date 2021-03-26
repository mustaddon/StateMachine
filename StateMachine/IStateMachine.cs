using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RandomSolutions
{
    public interface IStateMachine<TState, TEvent> : IStateMachine
    {
        TState Current { get; }

        IEnumerable<TState> GetStates(params object[] data);
        Task<IEnumerable<TState>> GetStatesAsync(params object[] data);

        IEnumerable<TEvent> GetEvents(params object[] data);
        Task<IEnumerable<TEvent>> GetEventsAsync(params object[] data);

        object Trigger(TEvent e, params object[] data);
        Task<object> TriggerAsync(TEvent e, params object[] data);

        bool JumpTo(TState state, params object[] data);
        Task<bool> JumpToAsync(TState state, params object[] data);

        void ResetTo(TState state);
        Task ResetToAsync(TState state);
    }

    public interface IStateMachine
    {
        void Reset();
        Task ResetAsync();
    }
}
