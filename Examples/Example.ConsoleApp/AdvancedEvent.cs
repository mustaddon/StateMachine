using FluentStateMachine;

namespace ConsoleApp;


public class AdvancedEvent(int id, string name) : AdvancedEvent<object, object>(id, name)
{
    public static readonly AdvancedEvent<string> E0 = new(0, "Event-0");
    public static readonly AdvancedEvent<int?, int?> E1 = new(1, "Event-1");
    public static readonly AdvancedEvent<bool> E2 = new(2, "Event-2");
    public static readonly AdvancedEvent<(int Arg1, string Arg2), string> E3 = new(3, "Event-3");
}

public class AdvancedEvent<TResult>(int id, string name) : AdvancedEvent<object, TResult>(id, name);

public class AdvancedEvent<TArgs, TResult>(int id, string name) : IFsmEvent<TArgs, TResult>, IAdvancedEvent
{
    public int Id { get; } = id;
    public string Name { get; } = name;

    public override int GetHashCode() => Id;
    public override bool Equals(object obj) => obj is IAdvancedEvent e ? e.Id == Id : obj is int id && id == Id;
    public override string ToString() => Name;
}

public interface IAdvancedEvent : IFsmEvent
{
    int Id { get; }
    string Name { get; }
}