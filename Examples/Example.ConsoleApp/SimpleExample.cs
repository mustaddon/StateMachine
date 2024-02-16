using FluentStateMachine;
using System;
using System.Threading.Tasks;


namespace Example.ConsoleApp;

public enum States { S1, S2, S3 }
public enum Events { E0, E1, E2, E3 }

internal class SimpleExample
{
    public static async Task Run()
    {
        Console.WriteLine($"=== SimpleExample Start ===\n");

        var fsm = new FsmBuilder<States, Events>(States.S1)
            .OnError(x => Console.WriteLine($"{x.Event}: On error ({x.Error})"))
            .OnTrigger(x => Console.WriteLine($"{x.Event}: On trigger"))
            .OnComplete(x => Console.WriteLine($"{x.Event}: On complete (triggered and state{(x.State == x.PrevState ? " NOT " : " ")}changed)"))
            .OnExit(x => Console.WriteLine($"{x.Event}: Exit state {x.State} to {x.NextState}"))
            .OnEnter(x => Console.WriteLine($"{x.Event}: Enter state {x.State} from {x.PrevState}"))
            .OnJump(x => Console.WriteLine($"{x.Event}: On jump to {x.State} from {x.PrevState}"))
            .OnReset(x => Console.WriteLine($"Reset to {x.State} from {x.PrevState}"))
            .On(Events.E0).Execute(x => "shared to all states")

            .State(States.S1)
                .On(Events.E1)
                    .Execute(x =>
                    {
                        Console.WriteLine($"{x.Event}: Execute with args: {x.Data}");
                        return (int)x.Data * 10; // some result
                    })
                .On(Events.E2).JumpTo(States.S2)
                .On(Events.E3).Execute(x => x.Data.ToString()).JumpTo(States.S3)
            .State(States.S2)
                .Enable(async x => { await Task.Delay(500); return true; })
                .On<int>(Events.E1) // with args cast 
                    .Execute(x => x.Data * 100)
                    .JumpTo(async x => { await Task.Delay(x.Data); return States.S1; })
            .State(States.S3)
                .OnEnter(x => Console.WriteLine($"{x.Event}: Final state !!!"))
                .On(Events.E0).Execute(x => $"overridden shared result !!!")
            .Build();


        Console.WriteLine($"Cast results: {await fsm.TriggerAsync<int?>(Events.E1, 10)}\n\n");

        var testEvents = new (Events, object?)[] {
            (Events.E2, null),
            (Events.E0, null),
            (Events.E1, 555),
            (Events.E3, (777, "test tuple")),
            (Events.E0, null)
        };

        foreach (var (e, data) in testEvents)
        {
            Console.WriteLine($"Current state: {fsm.Current}");
            Console.WriteLine($"Available events: {string.Join(", ", await fsm.GetAvailableEventsAsync(data))}");
            Console.WriteLine($"Result: {await fsm.TriggerAsync(e, data)}\n\n");
        }

        await fsm.ResetAsync();

        Console.WriteLine($"Done\n");
    }
}
