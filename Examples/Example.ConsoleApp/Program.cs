using FluentStateMachine;
using System;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        enum State { S1, S2, S3 }
        enum Event { E0, E1, E2, E3 }

        static async Task Main()
        {
            var fsm = new FsmBuilder<State, Event>(State.S1)
                .OnError(x => Console.WriteLine($"On error {x.Fsm.Current}: {x.Error}"))
                .OnTrigger(x => Console.WriteLine($"On trigger {x.Event}"))
                .OnComplete(x => Console.WriteLine($"On complete (triggered and state{(x.Fsm.Current == x.PrevState ? " NOT " : " ")}changed)"))
                .OnExit(x => { 
                    Console.WriteLine($"TOP Exit state {x.Fsm.Current} to {x.NextState}"); 
                    if (x.Fsm.Current == State.S1 && x.NextState != State.S3) x.Fsm.JumpTo(State.S3); 
                })
                .OnEnter(x => Console.WriteLine($"TOP Enter state {x.Fsm.Current} from {x.PrevState}"))
                .OnJump(x => Console.WriteLine($"On jump to {x.Fsm.Current} from {x.PrevState}"))
                .OnReset(x => Console.WriteLine($"On reset to {x.Fsm.Current} from {x.PrevState}"))
                .On(Event.E0).Execute(x => "shared to all states")

                .State(State.S1)
                    .OnExit(x => Console.WriteLine($"S1 Exit state {x.Fsm.Current} to {x.NextState}"))
                    .OnEnter(x => Console.WriteLine($"S1 Enter state {x.Fsm.Current} from {x.PrevState}"))
                    .On(Event.E0).JumpTo(State.S1)
                .State(State.S2)
                    .OnExit(x =>
                    {
                        Console.WriteLine($"S2 Exit state {x.Fsm.Current} to {x.NextState}");
                        if(x.NextState != State.S3) x.Fsm.JumpTo(State.S3);
                    })
                    .OnEnter(x => Console.WriteLine($"S2 Enter state {x.Fsm.Current} from {x.PrevState}"))
                    .On(Event.E0).JumpTo(State.S2)
                .State(State.S3)
                    .OnExit(x => Console.WriteLine($"S3 Exit state {x.Fsm.Current} to {x.NextState}"))
                    .OnEnter(x =>
                    {
                        Console.WriteLine($"S3 Enter state {x.Fsm.Current} from {x.PrevState}");
                        if(x.PrevState == State.S1) x.Fsm.JumpTo(State.S2);
                    })
                    .On(Event.E0).JumpTo(State.S3)
                .Build();

            var events = new[] { Event.E0, Event.E0, Event.E0 };

            foreach (var e in events)
            {
                Console.WriteLine($"Current state: {fsm.Current}");
                Console.WriteLine($"Available events: {string.Join(", ", fsm.GetAvailableEvents())}");
                Console.WriteLine($"Result: {await fsm.TriggerAsync(e)}\n\n");
            }

            await fsm.ResetAsync();
        }
    }
}
