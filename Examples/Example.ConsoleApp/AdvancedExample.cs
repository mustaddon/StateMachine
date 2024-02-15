using FluentStateMachine;
using System;
using System.Threading.Tasks;

namespace Example.ConsoleApp;

internal class AdvancedExample
{
    public static async Task Run()
    {
        Console.WriteLine($"=== AdvancedExample Start ===\n");

        var fsm = new FsmBuilder<States, IAdvancedEvent>(States.S1)
            .OnError(x => Console.WriteLine($"On error {x.CurrentState}: {x.Error}"))
            .OnTrigger(x => Console.WriteLine($"On trigger {x.Event}"))
            .OnComplete(x => Console.WriteLine($"On complete (triggered and state{(x.CurrentState == x.PrevState ? " NOT " : " ")}changed)"))
            .OnExit(x => Console.WriteLine($"Exit state {x.CurrentState} to {x.NextState}"))
            .OnEnter(x => Console.WriteLine($"Enter state {x.CurrentState} from {x.PrevState}"))
            .OnJump(x => Console.WriteLine($"On jump to {x.CurrentState} from {x.PrevState}"))
            .OnReset(x => Console.WriteLine($"On reset to {x.CurrentState} from {x.PrevState}"))
            .OnX(AdvancedEvents.E0).Execute(x => "shared to all states")

            .State(States.S1)
                .OnX(AdvancedEvents.E1)
                    .Execute(x =>
                    {
                        Console.WriteLine($"Execute {x.CurrentState}>{x.Event} with args cast and results type check");
                        return x.Data * 10; // some result
                    })
                .OnX(AdvancedEvents.E2).JumpTo(States.S2)
                .OnX(AdvancedEvents.E3).Execute(x => x.Data.ToString()).JumpTo(States.S3)
            .State(States.S2)
                .Enable(async x => { await Task.Delay(500); return true; })
                .OnX(AdvancedEvents.E1)
                    .Execute(x => x.Data * 100)
                    .JumpTo(async x => { await Task.Delay(500); return States.S1; })
            .State(States.S3)
                .OnEnter(x => Console.WriteLine($"Final state"))
                .OnX(AdvancedEvents.E0).Execute(x => "overridden shared event !!!")
            .Build();


        Console.WriteLine($"Check args type and cast results: {await fsm.TriggerAsyncX(AdvancedEvents.E1, 10)}\n\n");


        foreach (var (e, data) in new (IAdvancedEvent, object)[] {
            (AdvancedEvents.E1, 555),
            (AdvancedEvents.E2, null),
            (AdvancedEvents.E0, null),
            (AdvancedEvents.E1, 777),
            (AdvancedEvents.E3, (999, "test tuple")),
            (AdvancedEvents.E0, null)
        })
        {
            Console.WriteLine($"Current state: {fsm.Current}");
            Console.WriteLine($"Available events: {string.Join(", ", fsm.GetAvailableEvents(data))}");
            Console.WriteLine($"Result: {await fsm.TriggerAsync(e, data)}\n\n");
        }

        await fsm.ResetAsync();

        Console.WriteLine($"Done\n");
    }
}
