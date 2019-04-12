using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomSolutions
{
    public class StateMachine<TState,TEvent> : IStateMachine<TState, TEvent>
    {
        FsmModel<TState, TEvent> _model;

        public StateMachine(FsmModel<TState, TEvent> model)
        {
            _model = model;
            Current = model.Start;
        }
        
        public TState Current { get; private set; }

        public IEnumerable<TEvent> GetEvents(params object[] data)
        { 
            return _model.States[Current].Events
                .Where(x => x.Value.Enable?.Invoke(new FsmTriggerArgs<TState, TEvent>
                {
                    Fsm = this,
                    Event = x.Key,
                    Data = data,
                }) != false)
                .Select(x => x.Key);
        }

        public object Trigger(TEvent e, params object[] data)
        {
            var args = new FsmTriggerArgs<TState, TEvent>
            {
                Fsm = this,
                Event = e,
                Data = data,
            };

            _model.OnTrigger?.Invoke(args);

            var stateModel = _model.States[Current];

            if (!stateModel.Events.ContainsKey(e))
            {
                _model.OnError?.Invoke(new FsmException(_eventNotFound));
                return null;
            }

            var eventModel = stateModel.Events[e];
            
            if (eventModel.Enable?.Invoke(args) == false)
            {
                _model.OnError?.Invoke(new FsmException(_eventDisabled));
                return null;
            }

            _model.OnFire?.Invoke(args);

            var current = Current;
            var result = eventModel.Execute?.Invoke(args);

            if (object.Equals(current, Current) && eventModel.JumpTo != null)
                JumpTo(eventModel.JumpTo.Invoke(args));

            return result;
        }

        public bool JumpTo(TState next, params object[] data)
        {
            if (!_model.States.ContainsKey(next))
            {
                _model.OnError?.Invoke(new FsmException(_stateNextNotFound));
                return false;
            }

            var nextModel = _model.States[next];

            var args = new FsmEnterArgs<TState, TEvent>
            {
                Fsm = this,
                PrevState = Current,
                Data = data,
            };

            if (nextModel.Enable?.Invoke(args) == false)
            {
                _model.OnError?.Invoke(new FsmException(_stateNextDisabled));
                return false;
            }

             _model.States[Current].OnExit?.Invoke(new FsmExitArgs<TState, TEvent>
            {
                Fsm = this,
                NextState = next,
                Data = data,
            });

            Current = next;

            _model.OnChange?.Invoke(args);

            nextModel.OnEnter?.Invoke(args);

            return true;
        }

        const string _stateNextDisabled = "Next state disabled";
        const string _stateNextNotFound = "Next state not found";
        const string _eventDisabled = "Event disabled";
        const string _eventNotFound = "Event not found";
    }
    
}
