using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Example.ConsoleApp;

public class ExampleEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public States State { get; set; }
}


public static class MockEntityStore
{
    static readonly ConcurrentDictionary<int, ExampleEntity> _stored = new();

    public static Task<ExampleEntity> GetEntityById(int id)
    {
        return Task.FromResult(_stored.GetOrAdd(id, k => new()
        {
            Id = id,
            Name = $"Example #{id}",
            State = States.S1,
        }));
    }
}