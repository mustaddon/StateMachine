using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public IEnumerable<TState> GetStates(params object[] data)
        {
            return GetStatesAsync(data).Result;
        }

        public async Task<IEnumerable<TState>> GetStatesAsync(params object[] data)
        {
            var args = new FsmEnterArgs<TState, TEvent>
            {
                Fsm = this,
                PrevState = Current,
                Data = data,
            };

            var stateTasks = _model.States
                .Select(x => new { State = x.Key, Task = x.Value.Enable?.Invoke(args) })
                .ToList();

            await Task.WhenAll(stateTasks.Select(x => x.Task).Where(x => x != null)).ConfigureAwait(false);

            return stateTasks
                .Where(x => x.Task?.Result != false)
                .Select(x => x.State);
        }

        public IEnumerable<TEvent> GetEvents(params object[] data)
        {
            return GetEventsAsync(data).Result;
        }

        public async Task<IEnumerable<TEvent>> GetEventsAsync(params object[] data)
        {
            var stateModel = _model.States[Current];
            
            var eventTasks = _model.Events
                .Where(x => !stateModel.Events.ContainsKey(x.Key))
                .Concat(stateModel.Events)
                .Select(x => new
                {
                    Event = x.Key,
                    Task = x.Value.Enable?.Invoke(new FsmTriggerArgs<TState, TEvent>
                    {
                        Fsm = this,
                        Event = x.Key,
                        Data = data,
                    })
                })
                .ToList();

            await Task.WhenAll(eventTasks.Select(x => x.Task).Where(x => x != null)).ConfigureAwait(false);

            return eventTasks
                .Where(x => x.Task?.Result != false)
                .Select(x => x.Event);
        }

        public object Trigger(TEvent e, params object[] data)
        {
            return TriggerAsync(e, data).Result;
        }

        public async Task<object> TriggerAsync(TEvent e, params object[] data)
        {
            var args = new FsmTriggerArgs<TState, TEvent>
            {
                Fsm = this,
                Event = e,
                Data = data,
            };

            if (_model.OnTrigger != null)
                await _model.OnTrigger(args);

            var stateModel = _model.States[Current];
            FsmEventModel<TState, TEvent> eventModel;

            if (!stateModel.Events.TryGetValue(e, out eventModel) && !_model.Events.TryGetValue(e, out eventModel))
            {
                await _onError(_getErrorArgs(data, _eventNotFound, e));
                return null;
            }

            if (eventModel.Enable != null && !await eventModel.Enable(args))
            {
                await _onError(_getErrorArgs(data, _eventDisabled, e));
                return null;
            }

            if (_model.OnFire != null)
                await _model.OnFire(args);

            var result = eventModel.Execute == null ? null
                : await eventModel.Execute(args);

            if (eventModel.JumpTo != null)
            {
                var next = await eventModel.JumpTo(args);
                var done = await JumpToAsync(next, data);

                if (eventModel.Execute == null)
                    result = done;
            }

            if (_model.OnComplete != null)
                await _model.OnComplete(new FsmCompleteArgs<TState, TEvent>
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
            return JumpToAsync(next, data).Result;
        }

        public async Task<bool> JumpToAsync(TState next, params object[] data)
        {
            if (!_model.States.ContainsKey(next))
            {
                await _onError(_getErrorArgs(data, _stateNextNotFound, next));
                return false;
            }

            var nextModel = _model.States[next];

            var enterArgs = new FsmEnterArgs<TState, TEvent>
            {
                Fsm = this,
                PrevState = Current,
                Data = data,
            };

            if (nextModel.Enable != null && await nextModel.Enable(enterArgs) == false)
            {
                await _onError(_getErrorArgs(data, _stateNextDisabled, next));
                return false;
            }

            var exitArgs = new FsmExitArgs<TState, TEvent>
            {
                Fsm = this,
                NextState = next,
                Data = data,
            };

            if (_model.OnExit != null)
                await _model.OnExit(exitArgs);

            if (_model.States[Current].OnExit != null)
                await _model.States[Current].OnExit(exitArgs);

            lock (_locker)
                Current = next;

            if (_model.OnEnter != null)
                await _model.OnEnter(enterArgs);

            if (nextModel.OnEnter != null)
                await nextModel.OnEnter(enterArgs);

            if (_model.OnJump != null)
                await _model.OnJump(enterArgs);

            return true;
        }

        public void Reset() => ResetTo(_model.Start);
        public Task ResetAsync() => ResetToAsync(_model.Start);

        public void ResetTo(TState state)
        {
            ResetToAsync(state).Wait();
        }

        public async Task ResetToAsync(TState state)
        {
            var args = new FsmResetArgs<TState, TEvent>
            {
                Fsm = this,
                PrevState = Current,
            };

            lock (_locker)
                Current = state;

            if (_model.OnReset != null)
                await _model.OnReset(args);
        }

        async Task _onError(FsmErrorArgs<TState, TEvent> args)
        {
            if (_model.OnError != null)
                await _model.OnError(args);
        }

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
        object _locker = new object();

        const string _stateNextDisabled = "Next state '{0}' disabled";
        const string _stateNextNotFound = "Next state '{0}' not found";
        const string _eventDisabled = "Event '{0}' disabled";
        const string _eventNotFound = "Event '{0}' not found";
    }

}
