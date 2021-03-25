using System;

namespace RandomSolutions
{
    public class FsmBuilder<TState, TEvent>
    {
        public FsmBuilder(TState start) : this(new FsmModel<TState, TEvent> { Start = start })
        { }

        public FsmBuilder(FsmModel<TState, TEvent> model)
        {
            Model = model;
        }

        readonly FsmModel<TState, TEvent> Model;

        public FsmBuilder<TState, TEvent> OnReset(Action<FsmResetArgs<TState, TEvent>> action)
        {
            Model.OnReset = action;
            return this;
        }

        public FsmBuilder<TState, TEvent> OnExit(Action<FsmExitArgs<TState, TEvent>> action)
        {
            Model.OnExit = action;
            return this;
        }

        public FsmBuilder<TState, TEvent> OnEnter(Action<FsmEnterArgs<TState, TEvent>> action)
        {
            Model.OnEnter = action;
            return this;
        }

        public FsmBuilder<TState, TEvent> OnJump(Action<FsmEnterArgs<TState, TEvent>> action)
        {
            Model.OnJump = action;
            return this;
        }

        public FsmBuilder<TState, TEvent> OnTrigger(Action<FsmTriggerArgs<TState, TEvent>> action)
        {
            Model.OnTrigger = action;
            return this;
        }

        public FsmBuilder<TState, TEvent> OnFire(Action<FsmTriggerArgs<TState, TEvent>> action)
        {
            Model.OnFire = action;
            return this;
        }

        public FsmBuilder<TState, TEvent> OnComplete(Action<FsmCompleteArgs<TState, TEvent>> action)
        {
            Model.OnComplete = action;
            return this;
        }

        public FsmBuilder<TState, TEvent> OnError(Action<FsmErrorArgs<TState, TEvent>> action)
        {
            Model.OnError = action;
            return this;
        }

        public FsmStateConfig<TState, TEvent> State(TState state)
        {
            var stateModel = new FsmStateModel<TState, TEvent>();

            if (Model.States.ContainsKey(state))
                Model.States[state] = stateModel;
            else
                Model.States.Add(state, stateModel);

            return new FsmStateConfig<TState, TEvent>
            {
                Model = stateModel,
                Root = this,
            };
        }

        public IStateMachine<TState, TEvent> Build()
        {
            if (!Model.States.ContainsKey(Model.Start))
                throw new Exception(_startNotContains);

            return new StateMachine<TState, TEvent>(Model);
        }

        const string _startNotContains = "States collection is not contains start point";
    }
}
