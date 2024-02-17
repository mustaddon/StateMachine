namespace FluentStateMachine;

public interface IFsmEvent;
public interface IFsmEvent<TResult> : IFsmEvent;
public interface IFsmEvent<out TData, out TResult> : IFsmEvent;
public interface IFsmEventData<out TData, out TResult> : IFsmEvent<TData, TResult> where TData : IFsmEvent;

