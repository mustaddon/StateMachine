using FluentStateMachine;
using MediatR;

namespace Example.ConsoleApp;

public class MediatorRequest0 : IFsmEvent<MediatorRequest0, object>, IRequest<object>
{

}

public class MediatorRequest1 : IFsmEvent<MediatorRequest1, double>, IRequest<double>
{
    public int Num { get; set; }
}

public class MediatorRequest2 : IFsmEvent<MediatorRequest2, object>, IRequest<object>
{
    public bool Bit { get; set; }
}

public class MediatorRequest3 : IFsmEvent<MediatorRequest3, string>, IRequest<string>
{
    public int Num { get; set; }
    public string? Text { get; set; }
}

public class MediatorRequest4 : IFsmEvent<MediatorRequest4, object>, IRequest<object>
{
    public int Num { get; set; }
}
