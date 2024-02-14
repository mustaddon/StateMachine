namespace FluentStateMachine;

public interface IFsmEvent
{
}

public interface IFsmEvent<TData, TResult> : IFsmEvent
{
}

public interface IFsmEvent<TResult> : IFsmEvent<object, TResult>
{
}


