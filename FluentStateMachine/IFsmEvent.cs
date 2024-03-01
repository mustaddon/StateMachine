namespace FluentStateMachine;

public interface IFsmEventBase;
public interface IFsmEvent : IFsmEventBase;
public interface IFsmEvent<TResult> : IFsmEventBase;

