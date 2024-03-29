﻿using FluentStateMachine;
using System;


namespace Example.ConsoleApp;

internal class ReadmeExample2
{
    public static void Run()
    {
        Console.WriteLine($"=== ReadmeExample (type-based events) Start ===\n");

        var fsm = new FsmBuilder<States, Type>(States.S1)
            .OnEnter(x => Console.WriteLine($"State change to {x.State} from {x.PrevState}"))

            .State(States.S1)
                .On<Event1, string>().Execute(x => $"{x.Data.SomeProp} results")
                .On<Event2>().JumpTo(States.S2)

            .State(States.S2)
                .On<Event3>().JumpTo(States.S3)

            .State(States.S3)
                .OnEnter(x => Console.WriteLine($"Enter to final state"))

            .Build();


        Console.WriteLine(
            fsm.Trigger(new Event1 { 
                SomeProp = 123 
            }));

        fsm.Trigger(new Event2());

        fsm.Trigger(new Event3());

        Console.WriteLine($"Done\n");
    }


    class Event1 : IFsmEvent<string>
    {
        public int SomeProp { get; set; }
    }

    class Event2 : IFsmEvent;
    class Event3 : IFsmEvent;
}
