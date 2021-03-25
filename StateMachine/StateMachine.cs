using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomSolutions
{
    public class StateMachine<TState, TEvent> : IStateMachine<TState, TEvent>
    {
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

        public IEnumerable<TState> GetStates(params object[] data)
        {
            return _model.States
                .Where(x => x.Value.Enable?.Invoke(new FsmEnterArgs<TState, TEvent>
                {
                    Fsm = this,
                    PrevState = Current,
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
                _model.OnError?.Invoke(_getErrorArgs(data, _eventNotFound, e));
                return null;
            }

            var eventModel = stateModel.Events[e];

            if (eventModel.Enable?.Invoke(args) == false)
            {
                _model.OnError?.Invoke(_getErrorArgs(data, _eventDisabled, e));
                return null;
            }

            _model.OnFire?.Invoke(args);

            var result = eventModel.Execute?.Invoke(args);

            if (eventModel.JumpTo != null)
            {
                var done = JumpTo(eventModel.JumpTo.Invoke(args), data);
                if (eventModel.Execute == null)
                    result = done;
            }

            _model.OnComplete?.Invoke(new FsmCompleteArgs<TState, TEvent>
            {
                Fsm = this,
                Event = e,
                Data = data,
                Result = result,
            });

            return result;
        }

        public bool JumpTo(TState next, params object[] data)
        {
            if (!_model.States.ContainsKey(next))
            {
                _model.OnError?.Invoke(_getErrorArgs(data, _stateNextNotFound, next));
                return false;
            }

            var nextModel = _model.States[next];

            var enterArgs = new FsmEnterArgs<TState, TEvent>
            {
                Fsm = this,
                PrevState = Current,
                Data = data,
            };

            if (nextModel.Enable?.Invoke(enterArgs) == false)
            {
                _model.OnError?.Invoke(_getErrorArgs(data, _stateNextDisabled, next));
                return false;
            }

            var exitArgs = new FsmExitArgs<TState, TEvent>
            {
                Fsm = this,
                NextState = next,
                Data = data,
            };

            _model.OnExit?.Invoke(exitArgs);

            _model.States[Current].OnExit?.Invoke(exitArgs);

            Current = next;

            _model.OnEnter?.Invoke(enterArgs);

            nextModel.OnEnter?.Invoke(enterArgs);

            _model.OnJump?.Invoke(enterArgs);

            return true;
        }


        public void ResetTo(TState state)
        {
            var args = new FsmResetArgs<TState, TEvent>
            {
                Fsm = this,
                PrevState = Current,
            };

            Current = state;
            _model.OnReset?.Invoke(args);
        }

        public void Reset() => ResetTo(_model.Start);


        FsmErrorArgs<TState, TEvent> _getErrorArgs(object[] data, string message, params object[] formatArgs)
        {
            return new FsmErrorArgs<TState, TEvent>
            {
                Fsm = this,
                Data = data,
                Message = string.Format(message, formatArgs),
            };
        }

        readonly FsmModel<TState, TEvent> _model;

        const string _stateNextDisabled = "Next state '{0}' disabled";
        const string _stateNextNotFound = "Next state '{0}' not found";
        const string _eventDisabled = "Event '{0}' disabled";
        const string _eventNotFound = "Event '{0}' not found";
    }

}
