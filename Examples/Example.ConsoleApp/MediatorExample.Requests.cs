using FluentStateMachine.MediatR;

namespace Example.ConsoleApp;

public abstract class ExampleFactoryRequest
{
    public int MyEntityId { get; set; }
}

public class MediatorRequest0 : ExampleFactoryRequest, IFsmRequest<string>;

public class MediatorRequest1 : ExampleFactoryRequest, IFsmRequest<double>
{
    public int Num { get; set; }
}

public class MediatorRequest2 : ExampleFactoryRequest, IFsmRequest
{
    public bool Bit { get; set; }
}

public class MediatorRequest3 : ExampleFactoryRequest, IFsmRequest<string>
{
    public int Num { get; set; }
    public string? Text { get; set; }
}

public class MediatorRequest4 : ExampleFactoryRequest, IFsmRequest<object>
{
    public int Num { get; set; }
}
