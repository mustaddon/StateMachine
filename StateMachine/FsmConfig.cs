using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RandomSolutions
{
    public class FsmConfig<TState, TEvent>
    {
        internal FsmBuilder<TState, TEvent> Root;

        public FsmStateConfig<TState, TEvent> State(TState state)
            => Root.State(state);

        public IStateMachine<TState, TEvent> Build()
            => Root.Build();
    }

    public class FsmStateConfig<TState, TEvent> : FsmConfig<TState, TEvent>
    {
        internal FsmStateModel<TState, TEvent> Model;

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
                Root = Root,
                Parent = this,
            };
        }

        public FsmStateConfig<TState, TEvent> OnEnter(Action<FsmEnterArgs<TState, TEvent>> action)
        {
            return OnEnter(x => { action(x); return FrameworkExt.CompletedTask; });
        }

        public FsmStateConfig<TState, TEvent> OnEnter(Func<FsmEnterArgs<TState, TEvent>, Task> action)
        {
            Model.OnEnter = action;
            return this;
        }

        public FsmStateConfig<TState, TEvent> OnExit(Action<FsmExitArgs<TState, TEvent>> action)
        {
            return OnExit(x => { action(x); return FrameworkExt.CompletedTask; });
        }

        public FsmStateConfig<TState, TEvent> OnExit(Func<FsmExitArgs<TState, TEvent>, Task> action)
        {
            Model.OnExit = action;
            return this;
        }

        public FsmStateConfig<TState, TEvent> Enable(Func<FsmEnterArgs<TState, TEvent>, bool> fn)
        {
            return Enable(x => Task.FromResult(fn(x)));
        }

        public FsmStateConfig<TState, TEvent> Enable(Func<FsmEnterArgs<TState, TEvent>, Task<bool>> fn)
        {
            Model.Enable = fn;
            return this;
        }
    }

    public class FsmEventConfig<TState, TEvent> : FsmConfig<TState, TEvent>
    {
        internal FsmEventModel<TState, TEvent> Model;

        internal FsmStateConfig<TState, TEvent> Parent;

        public FsmEventConfig<TState, TEvent> On(TEvent e) => Parent.On(e);

        public FsmEventConfig<TState, TEvent> Execute(Func<FsmTriggerArgs<TState, TEvent>, object> fn)
        {
            return Execute(x => Task.FromResult(fn(x)));
        }

        public FsmEventConfig<TState, TEvent> Execute(Func<FsmTriggerArgs<TState, TEvent>, Task<object>> fn)
        {
            Model.Execute = fn;
            return this;
        }

        public FsmEventConfig<TState, TEvent> Enable(Func<FsmTriggerArgs<TState, TEvent>, bool> fn)
        {
            return Enable(x => Task.FromResult(fn(x)));
        }

        public FsmEventConfig<TState, TEvent> Enable(Func<FsmTriggerArgs<TState, TEvent>, Task<bool>> fn)
        {
            Model.Enable = fn;
            return this;
        }

        public FsmEventConfig<TState, TEvent> JumpTo(TState state)
        {
            return JumpTo(x => Task.FromResult(state));
        }

        public FsmEventConfig<TState, TEvent> JumpTo(Func<FsmTriggerArgs<TState, TEvent>, TState> fn)
        {
            return JumpTo(x => Task.FromResult(fn(x)));
        }

        public FsmEventConfig<TState, TEvent> JumpTo(Func<FsmTriggerArgs<TState, TEvent>, Task<TState>> fn)
        {
            Model.JumpTo = fn;
            return this;
        }
    }
}
