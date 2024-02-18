using FluentStateMachine;
using System;
using System.Threading.Tasks;

namespace Example.ConsoleApp;

internal class ExtendedExample
{
    public static async Task Run()
    {
        Console.WriteLine($"=== ExtendedExample Start ===\n");

        var fsm = new FsmBuilder<States, IExtendedEvent>(States.S1)
            .OnTrigger(x => Console.WriteLine($"{x.Event}: On trigger"))
            .OnComplete(x => Console.WriteLine($"{x.Event}: On complete (triggered and state{(x.State == x.PrevState ? " NOT " : " ")}changed)"))
            .OnExit(x => Console.WriteLine($"{x.Event}: Exit state {x.State} to {x.NextState}"))
            .OnEnter(x => Console.WriteLine($"{x.Event}: Enter state {x.State} from {x.PrevState}"))
            .OnJump(x => Console.WriteLine($"{x.Event}: On jump to {x.State} from {x.PrevState}"))
            .OnReset(x => Console.WriteLine($"Reset to {x.State} from {x.PrevState}"))
            .OnX(ExtendedEvents.E0).Execute(x => "shared to all states")

            .State(States.S1)
                .OnX(ExtendedEvents.E1)
                    .Execute(x =>
                    {
                        Console.WriteLine($"{x.Event}: Execute with args cast and results type check");
                        return x.Data * 10; // some result
                    })
                .OnX(ExtendedEvents.E2).JumpTo(States.S2)
                .OnX(ExtendedEvents.E3).Execute(x => x.Data.ToString()).JumpTo(States.S3)

            .State(States.S2)
                //.OnEnter(x => x.Fsm.JumpTo(States.S3)) // test skip state
                .Enable(async x => { await Task.Delay(500); return true; })
                .OnX(ExtendedEvents.E1)
                    .Execute(x => x.Data * 100)
                    .JumpTo(async x => { await Task.Delay(500); return States.S1; })

            .State(States.S3)
                .OnEnter(x => Console.WriteLine($"{x.Event}: Final state !!!"))
                .OnX(ExtendedEvents.E0).Execute(x => $"overridden shared result !!!")

            .Build();


        Console.WriteLine($"Check args type and cast results: {await fsm.TriggerAsyncX(ExtendedEvents.E1, 10)}\n\n");

        var testEvents = new (IExtendedEvent, object?)[] {
            (ExtendedEvents.E2, null),
            (ExtendedEvents.E0, null),
            (ExtendedEvents.E1, 555),
            (ExtendedEvents.E3, (777, "test tuple")),
            (ExtendedEvents.E0, null)
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
