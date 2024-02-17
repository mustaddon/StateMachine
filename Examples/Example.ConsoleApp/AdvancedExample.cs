﻿using FluentStateMachine;
using System;
using System.Threading.Tasks;

namespace Example.ConsoleApp;

internal class AdvancedExample
{
    public static async Task Run()
    {
        Console.WriteLine($"=== AdvancedExample Start ===\n");

        var fsm = new FsmBuilder<States, IAdvancedEvent>(States.S1)
            .OnError(x => Console.WriteLine($"{x.Event}: On error ({x.Error})"))
            .OnTrigger(x => Console.WriteLine($"{x.Event}: On trigger"))
            .OnComplete(x => Console.WriteLine($"{x.Event}: On complete (triggered and state{(x.State == x.PrevState ? " NOT " : " ")}changed)"))
            .OnExit(x => Console.WriteLine($"{x.Event}: Exit state {x.State} to {x.NextState}"))
            .OnEnter(x => Console.WriteLine($"{x.Event}: Enter state {x.State} from {x.PrevState}"))
            .OnJump(x => Console.WriteLine($"{x.Event}: On jump to {x.State} from {x.PrevState}"))
            .OnReset(x => Console.WriteLine($"Reset to {x.State} from {x.PrevState}"))
            .OnX(AdvancedEvents.E0).Execute(x => "shared to all states")

            .State(States.S1)
                .OnX(AdvancedEvents.E1)
                    .Execute(x =>
                    {
                        Console.WriteLine($"{x.Event}: Execute with args cast and results type check");
                        return x.Data * 10; // some result
                    })
                .OnX(AdvancedEvents.E2).JumpTo(States.S2)
                .OnX(AdvancedEvents.E3).Execute(x => x.Data.ToString()).JumpTo(States.S3)

            .State(States.S2)
                //.OnEnter(x => x.Fsm.JumpTo(States.S3)) // test skip state
                .Enable(async x => { await Task.Delay(500); return true; })
                .OnX(AdvancedEvents.E1)
                    .Execute(x => x.Data * 100)
                    .JumpTo(async x => { await Task.Delay(500); return States.S1; })

            .State(States.S3)
                .OnEnter(x => Console.WriteLine($"{x.Event}: Final state !!!"))
                .OnX(AdvancedEvents.E0).Execute(x => $"overridden shared result !!!")

            .Build();


        Console.WriteLine($"Check args type and cast results: {await fsm.TriggerAsyncX(AdvancedEvents.E1, 10)}\n\n");

        var testEvents = new (IAdvancedEvent, object?)[] {
            (AdvancedEvents.E2, null),
            (AdvancedEvents.E0, null),
            (AdvancedEvents.E1, 555),
            (AdvancedEvents.E3, (777, "test tuple")),
            (AdvancedEvents.E0, null)
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
