using FluentStateMachine;

namespace Example.ConsoleApp;


public static class AdvancedEvents
{
    public static readonly AdvancedEvent<object, string> E0 = new(0);
    public static readonly AdvancedEvent<int, int> E1 = new(1);
    public static readonly AdvancedEvent<object, bool> E2 = new(2);
    public static readonly AdvancedEvent<(int Arg1, string Arg2), string> E3 = new(3);
    public static readonly AdvancedEvent<int, object> E4 = new(4);
}

public class AdvancedEvent<TData, TResult>(int id) : IAdvancedEvent, IFsmEvent<TData, TResult>
{ 
    public int Id { get; } = id;
    public override int GetHashCode() => Id;
    public override bool Equals(object? obj) => obj is IAdvancedEvent e ? e.Id == Id : obj is int id && id == Id;
    public override string ToString() => $"AdvEvent-{Id}";
}

public interface IAdvancedEvent : IFsmEvent
{
    int Id { get; }
}