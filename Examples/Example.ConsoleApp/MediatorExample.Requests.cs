namespace Example.ConsoleApp;

public interface IUnifiedRequest<TResult> : MediatR.IRequest<TResult>,  FluentStateMachine.IFsmEvent<TResult>;


public class MediatorRequest0 : IUnifiedRequest<object>;

public class MediatorRequest1 : IUnifiedRequest<double>
{
    public int Num { get; set; }
}

public class MediatorRequest2 : IUnifiedRequest<object>
{
    public bool Bit { get; set; }
}

public class MediatorRequest3 : IUnifiedRequest<string>
{
    public int Num { get; set; }
    public string? Text { get; set; }
}

public class MediatorRequest4 : IUnifiedRequest<object>
{
    public int Num { get; set; }
}
