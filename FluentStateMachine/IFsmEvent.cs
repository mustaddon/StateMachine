namespace FluentStateMachine;

public interface IFsmEvent
{
}

public interface IFsmEvent<TArgs, TResult> : IFsmEvent
{
}

public interface IFsmEvent<TResult> : IFsmEvent<object, TResult>
{
}


