using FluentStateMachine;
using System;
using System.Threading.Tasks;


namespace Example.ConsoleApp;

enum States { S1, S2, S3 }
enum Events { E0, E1, E2, E3 }

internal class SimpleExample
{
    public static async Task Run()
    {
        Console.WriteLine($"=== SimpleExample Start ===\n");

        var fsm = new FsmBuilder<States, Events>(States.S1)
            .OnError(x => Console.WriteLine($"On error {x.CurrentState}: {x.Error}"))
            .OnTrigger(x => Console.WriteLine($"On trigger {x.Event}"))
            .OnComplete(x => Console.WriteLine($"On complete (triggered and state{(x.CurrentState == x.PrevState ? " NOT " : " ")}changed)"))
            .OnExit(x => Console.WriteLine($"Exit state {x.CurrentState} to {x.NextState}"))
            .OnEnter(x => Console.WriteLine($"Enter state {x.CurrentState} from {x.PrevState}"))
            .OnJump(x => Console.WriteLine($"On jump to {x.CurrentState} from {x.PrevState}"))
            .OnReset(x => Console.WriteLine($"On reset to {x.CurrentState} from {x.PrevState}"))
            .On(Events.E0).Execute(x => "shared to all states")

            .State(States.S1)
                .On(Events.E1)
                    .Execute(x =>
                    {
                        Console.WriteLine($"Execute {x.CurrentState}>{x.Event} with args: {x.Data}");
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
                .OnEnter(x => Console.WriteLine($"Final state"))
                .On(Events.E0).Execute(x => "overridden shared event !!!")
            .Build();


        Console.WriteLine($"Cast results: {await fsm.TriggerAsync<int?>(Events.E1, 10)}\n\n");


        foreach (var (e, data) in new (Events, object)[] {
            (Events.E1, 555),
            (Events.E2, null),
            (Events.E0, null),
            (Events.E1, 777),
            (Events.E3, (999, "test tuple")),
            (Events.E0, null)
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
