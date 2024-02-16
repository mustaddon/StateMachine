using FluentStateMachine;
using System;
using MediatR;

namespace Example.ConsoleApp;

public interface IMediatorEvent : IFsmEvent;
public interface IMediatorEvent<out TData, out TResult> : IMediatorEvent, IFsmEventData<TData, TResult>, IRequest<TResult>  where TData : IMediatorEvent;

public abstract class MediatorEventBaseX<TData, TResult>(int id) : AdvancedEventBase(id), IMediatorEvent<TData, TResult> where TData : IMediatorEvent
{
    public static readonly TData Value = Activator.CreateInstance<TData>();
}


public class MediatorEvent0() : MediatorEventBaseX<MediatorEvent0, string>(0)
{
    public string? Text { get; set; }
}

public class MediatorEvent1() : MediatorEventBaseX<MediatorEvent1, int>(1)
{
    public int Num { get; set; }
}

public class MediatorEvent2() : MediatorEventBaseX<MediatorEvent2, bool>(2)
{
    public bool Bit { get; set; }
}

public class MediatorEvent3() : MediatorEventBaseX<MediatorEvent3, string>(3)
{
    public int Num { get; set; }
    public string? Text { get; set; }
}

public class MediatorEvent4() : MediatorEventBaseX<MediatorEvent4, object>(4)
{
    public int Num { get; set; }
}
