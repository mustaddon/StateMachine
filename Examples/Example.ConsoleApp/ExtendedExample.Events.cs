using FluentStateMachine;

namespace Example.ConsoleApp;


public static class ExtendedEvents
{
    public static readonly ExtendedEvent<object, string> E0 = new(0);
    public static readonly ExtendedEvent<int, int> E1 = new(1);
    public static readonly ExtendedEvent<object, bool> E2 = new(2);
    public static readonly ExtendedEvent<(int Arg1, string Arg2), string> E3 = new(3);
    public static readonly ExtendedEvent<object?, object> E4 = new(4);
}

public class ExtendedEvent<TData, TResult>(int id) : IExtendedEvent, IFsmEvent<TData, TResult>
{ 
    public int Id { get; } = id;
    public override int GetHashCode() => Id;
    public override bool Equals(object? obj) => obj is IExtendedEvent e ? e.Id == Id : obj is int id && id == Id;
    public override string ToString() => $"ExtEvent-{Id}";
}

public interface IExtendedEvent : IFsmEvent
{
    int Id { get; }
}