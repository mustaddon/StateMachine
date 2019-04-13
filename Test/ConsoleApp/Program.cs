using RandomSolutions;
using System;
using System.Collections.Generic;

namespace ConsoleApp
{
    class Program
    {
        enum State { S1, S2, S3 }
        enum Event { E1, E2, E3 }


        static void Main(string[] args)
        {
            var fsm = new FsmBuilder<State, Event>(State.S1)
                .OnChange(x => Console.WriteLine($"On state change to {x.Fsm.Current} from {x.PrevState}"))
                .OnTrigger(x => Console.WriteLine($"On trigger {x.Event}"))
                .OnError(x => Console.WriteLine($"On error {x.Fsm.Current}: {x.Message}"))
                .State(State.S1)
                    .OnEnter(_consoleWrite)
                    .OnExit(_consoleWrite)
                    .On(Event.E1).Execute(x => { Console.WriteLine($"Execute {x.Fsm.Current}>{x.Event}"); return "some data"; })
                    .On(Event.E2).JumpTo(State.S2)
                    .On(Event.E3).JumpTo(State.S3)
                .State(State.S2)
                    .OnEnter(_consoleWrite)
                    .OnExit(_consoleWrite)
                    .On(Event.E1).JumpTo(State.S1)
                .State(State.S3)
                    .OnEnter(_consoleWrite)
                    .OnExit(_consoleWrite)
                .Build();
            
            foreach (var e in new[] { Event.E1, Event.E2, Event.E1, Event.E3, Event.E1 })
            {
                Console.WriteLine($"{fsm.Current}: " + string.Join(", ", fsm.GetEvents()));
                Console.WriteLine("Result: " + fsm.Trigger(e));
                Console.WriteLine();
            }
        }



        static void _consoleWrite(FsmEnterArgs<State, Event> x) 
            => Console.WriteLine($"Enter state {x.Fsm.Current} from {x.PrevState}");

        static void _consoleWrite(FsmExitArgs<State, Event> x)
            => Console.WriteLine($"Exit state {x.Fsm.Current} to {x.NextState}");
    }
}
