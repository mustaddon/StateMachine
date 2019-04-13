using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomSolutions
{
    public class FsmBuilder<TState, TEvent>
    {
        public FsmBuilder(TState start)
        {
            _model = new FsmModel<TState, TEvent> {
                Start = start,
                States = new Dictionary<TState, FsmStateModel<TState, TEvent>>(),
            };
        }

        public FsmBuilder<TState, TEvent> OnError(Action<FsmException> action)
        {
            _model.OnError = action;
            return this;
        }

        public FsmBuilder<TState, TEvent> OnChange(Action<FsmEnterArgs<TState, TEvent>> action)
        {
            _model.OnChange = action;
            return this;
        }

        public FsmBuilder<TState, TEvent> OnTrigger(Action<FsmTriggerArgs<TState, TEvent>> action)
        {
            _model.OnTrigger = action;
            return this;
        }

        public FsmBuilder<TState, TEvent> OnFire(Action<FsmTriggerArgs<TState, TEvent>> action)
        {
            _model.OnFire = action;
            return this;
        }

        public FsmStateConfig<TState, TEvent> State(TState state)
        {
            var stateModel = new FsmStateModel<TState, TEvent>();

            if (_model.States.ContainsKey(state))
                _model.States[state] = stateModel;
            else
                _model.States.Add(state, stateModel);

            return new FsmStateConfig<TState, TEvent>
            {
                Model = stateModel,
                Root = this,
            };
        }

        public IStateMachine<TState, TEvent> Build() => new StateMachine<TState, TEvent>(BuildModel());

        public FsmModel<TState, TEvent> BuildModel()
        {
            if (!_model.States.ContainsKey(_model.Start))
                throw new Exception(_startNotContains);

            return _model;
        }

        FsmModel<TState, TEvent> _model;
        const string _startNotContains = "States collection is not contains start point";
    }
}
