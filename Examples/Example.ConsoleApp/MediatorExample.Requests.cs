using FluentStateMachine;
using MediatR;

namespace Example.ConsoleApp;

public class MediatorRequest0 : IFsmEventData<MediatorRequest0, object>, IRequest<object>
{

}

public class MediatorRequest1 : IFsmEventData<MediatorRequest1, double>, IRequest<double>
{
    public int Num { get; set; }
}

public class MediatorRequest2 : IFsmEventData<MediatorRequest2, object>, IRequest<object>
{
    public bool Bit { get; set; }
}

public class MediatorRequest3 : IFsmEventData<MediatorRequest3, string>, IRequest<string>
{
    public int Num { get; set; }
    public string? Text { get; set; }
}

public class MediatorRequest4 : IFsmEventData<MediatorRequest4, object>, IRequest<object>
{
    public int Num { get; set; }
}
