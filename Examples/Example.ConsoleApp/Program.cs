using FluentStateMachine;
using System;
using System.Threading.Tasks;

using IEvent = ConsoleApp.EnumEvent;
using Events = ConsoleApp.EnumEvent;

//using IEvent = ConsoleApp.IAdvancedEvent;
//using Events = ConsoleApp.AdvancedEvent;


namespace ConsoleApp;

class Program
{
    enum States { S1, S2, S3 }

    static async Task Main()
    {
        var fsm = new FsmBuilder<States, IEvent>(States.S1)
            .OnError(x => Console.WriteLine($"On error {x.Fsm.Current}: {x.Error}"))
            .OnTrigger(x => Console.WriteLine($"On trigger {x.Event}"))
            .OnComplete(x => Console.WriteLine($"On complete (triggered and state{(x.Fsm.Current == x.PrevState ? " NOT " : " ")}changed)"))
            .OnExit(x => Console.WriteLine($"Exit state {x.Fsm.Current} to {x.NextState}"))
            .OnEnter(x => Console.WriteLine($"Enter state {x.Fsm.Current} from {x.PrevState}"))
            .OnJump(x => Console.WriteLine($"On jump to {x.Fsm.Current} from {x.PrevState}"))
            .OnReset(x => Console.WriteLine($"On reset to {x.Fsm.Current} from {x.PrevState}"))
            .On(Events.E0).Execute(x => "shared to all states")

            .State(States.S1)
                .On(Events.E1)
                    .Execute(x =>
                    {
                        Console.WriteLine($"Execute {x.Fsm.Current}>{x.Event} with args: {x.Data}");
                        return (int?)x.Data * 10; // some result
                    })
                .On(Events.E2).JumpTo(States.S2)
                .On(Events.E3).Execute(x => x.Data.ToString()).JumpTo(States.S3)
            .State(States.S2)
                .Enable(async x => { await Task.Delay(500); return true; })
                .On(Events.E1)
                    .Execute(x => (int?)x.Data * 100)
                    .JumpTo(async x => { await Task.Delay(500); return States.S1; })
            .State(States.S3)
                .OnEnter(x => Console.WriteLine($"Final state"))
                .On(Events.E0).Execute(x => "overridden shared event !!!")
            .Build();


        //Console.WriteLine($"Result with cast: {await fsm.TriggerAsync(Events.E1, 100)}\n\n"); // for AdvancedEvents (not enum)


        foreach (var (e, data) in new (IEvent, object)[] {
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
    }
}
