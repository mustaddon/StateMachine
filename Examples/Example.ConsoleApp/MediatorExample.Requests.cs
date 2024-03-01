namespace Example.ConsoleApp;

public interface IUnifiedRequest<TResult> : MediatR.IRequest<TResult>,  FluentStateMachine.IFsmEvent<TResult>;
public interface IUnifiedRequest : MediatR.IRequest, FluentStateMachine.IFsmEvent;


public class MediatorRequest0 : IUnifiedRequest<string>;

public class MediatorRequest1 : IUnifiedRequest<double>
{
    public int Num { get; set; }
}

public class MediatorRequest2 : IUnifiedRequest
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
