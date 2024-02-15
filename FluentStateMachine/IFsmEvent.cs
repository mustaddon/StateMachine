namespace FluentStateMachine;

public interface IFsmEvent;
public interface IFsmEvent<TData, TResult> : IFsmEvent;