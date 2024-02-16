using FluentStateMachine;
using System;


namespace Example.ConsoleApp;

internal class ReadmeExample
{
    public static void Run()
    {
        Console.WriteLine($"=== ReadmeExample Start ===\n");

        var fsm = new FsmBuilder<States, Events>(States.S1)
            .OnEnter(x => Console.WriteLine($"State change to {x.Fsm.Current} from {x.PrevState}"))
            .State(States.S1)
                .On(Events.E1).Execute(x => { /* some operations */ return "some data"; })
                .On(Events.E2).JumpTo(States.S2)
            .State(States.S2)
                .On(Events.E3).Enable(x => /* some conditions */ true).JumpTo(States.S3)
            .State(States.S3)
                .OnEnter(x => Console.WriteLine($"Enter to final state"))
            .Build();


        fsm.Trigger(Events.E2);
        fsm.Trigger(Events.E3);

        Console.WriteLine($"Done\n");
    }
}
