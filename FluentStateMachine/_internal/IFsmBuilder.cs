namespace FluentStateMachine._internal;


internal interface IFsmBuilder<out TState, out TEvent> : IFsmState<TState>, IFsmEvent<TEvent>;
internal interface IFsmState<out TState>;
internal interface IFsmEvent<out TEvent>;
