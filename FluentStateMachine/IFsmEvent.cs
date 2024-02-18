namespace FluentStateMachine;

public interface IFsmEvent;
public interface IFsmEvent<TResult> : IFsmEvent;
public interface IFsmEvent<TData, TResult> : IFsmEvent;
public interface IFsmEventData<TData, TResult> : IFsmEvent<TData, TResult> where TData : IFsmEvent;

