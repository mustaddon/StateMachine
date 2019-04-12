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
                .OnChange(x => Console.WriteLine($"on state change to {x.Fsm.Current} from {x.PrevState}"))
                .OnTrigger(x => Console.WriteLine($"on trigger {x.Event}"))
                .OnError(x => Console.WriteLine($"on error {x}"))
                .State(State.S1)
                    .OnEnter(x => Console.WriteLine($"enter state {x.Fsm.Current} from {x.PrevState}"))
                    .OnExit(x => Console.WriteLine($"exit state {x.Fsm.Current} to {x.NextState}"))
                    .On(Event.E1).JumpTo(State.S1)
                    .On(Event.E2).Execute(x => x.Fsm.JumpTo(State.S2))
                    .On(Event.E3).JumpTo(State.S3)
                .State(State.S2)
                    .OnEnter(x => Console.WriteLine($"enter state {x.Fsm.Current} from {x.PrevState}"))
                    .OnExit(x => Console.WriteLine($"exit state {x.Fsm.Current} to {x.NextState}"))
                    .On(Event.E1).JumpTo(State.S1)
                .State(State.S3)
                    .OnEnter(x => Console.WriteLine($"enter state {x.Fsm.Current} from {x.PrevState}"))
                    .OnExit(x => Console.WriteLine($"exit state {x.Fsm.Current} to {x.NextState}"))
                .Build();
            

            foreach (var e in new[] { Event.E2, Event.E1, Event.E3, Event.E1 })
            {
                Console.WriteLine($"{fsm.Current}: " + string.Join(", ", fsm.GetEvents()));
                Console.WriteLine("result: " + fsm.Trigger(e));
                Console.WriteLine();
            }
        }
    }
}
