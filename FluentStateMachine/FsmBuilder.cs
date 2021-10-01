using System;
using System.Threading.Tasks;

namespace FluentStateMachine
{
    public class FsmBuilder<TState, TEvent>
    {
        public FsmBuilder(TState start) : this(new FsmModel<TState, TEvent> { Start = start })
        { }

        public FsmBuilder(FsmModel<TState, TEvent> model)
        {
            Model = model;
        }

        private readonly FsmModel<TState, TEvent> Model;

        public FsmBuilder<TState, TEvent> OnReset(Func<FsmResetArgs<TState, TEvent>, Task> action)
        {
            Model.OnReset = action;
            return this;
        }

        public FsmBuilder<TState, TEvent> OnExit(Func<FsmExitArgs<TState, TEvent>, Task> action)
        {
            Model.OnExit = action;
            return this;
        }

        public FsmBuilder<TState, TEvent> OnEnter(Func<FsmEnterArgs<TState, TEvent>, Task> action)
        {
            Model.OnEnter = action;
            return this;
        }

        public FsmBuilder<TState, TEvent> OnJump(Func<FsmEnterArgs<TState, TEvent>, Task> action)
        {
            Model.OnJump = action;
            return this;
        }

        public FsmBuilder<TState, TEvent> OnTrigger(Func<FsmTriggerArgs<TState, TEvent>, Task> action)
        {
            Model.OnTrigger = action;
            return this;
        }

        public FsmBuilder<TState, TEvent> OnFire(Func<FsmTriggerArgs<TState, TEvent>, Task> action)
        {
            Model.OnFire = action;
            return this;
        }

        public FsmBuilder<TState, TEvent> OnComplete(Func<FsmCompleteArgs<TState, TEvent>, Task> action)
        {
            Model.OnComplete = action;
            return this;
        }

        public FsmBuilder<TState, TEvent> OnError(Func<FsmErrorArgs<TState, TEvent>, Task> action)
        {
            Model.OnError = action;
            return this;
        }

        public FsmEventConfig<TState, TEvent> On(TEvent e)
        {
            var eventModel = new FsmEventModel<TState, TEvent>();

            if (Model.Events.ContainsKey(e))
                Model.Events[e] = eventModel;
            else
                Model.Events.Add(e, eventModel);

            return new FsmEventConfig<TState, TEvent>
            {
                Model = eventModel,
                Root = this,
            };
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

        private const string _startNotContains = "States collection is not contains start point";
    }

}
