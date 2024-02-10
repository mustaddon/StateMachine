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
                .OnComplete(x => Console.WriteLine($"On complete (triggered and state{(x.Fsm.Current == x.PrevState ? " not " : " ")}changed)"))
                .OnExit(x => Console.WriteLine($"Exit state {x.Fsm.Current} to {x.NextState}"))
                .OnEnter(x => Console.WriteLine($"Enter state {x.Fsm.Current} from {x.PrevState}"))
                .OnJump(x => Console.WriteLine($"On jump to {x.Fsm.Current} from {x.PrevState}"))
                .OnReset(x => Console.WriteLine($"On reset to {x.Fsm.Current} from {x.PrevState}"))
                .On(Event.E0).Execute(x => "shared to all states")

                .State(State.S1)
                    .On(Event.E1)
                        .Execute(async x =>
                        {
                            Console.WriteLine($"Execute {x.Fsm.Current}>{x.Event}");
                            await Task.Delay(1000);
                            return "some data";
                        })

                    .On(Event.E2).JumpTo(State.S2)
                    .On(Event.E3).JumpTo(State.S3)
                .State(State.S2)
                    .Enable(async x => { await Task.Delay(1000); return true; })
                    .On(Event.E1).JumpTo(async x => { await Task.Delay(1000); return State.S1; })
                .State(State.S3)
                    .OnEnter(x => Console.WriteLine($"Final state"))
                    .On(Event.E0).Execute(x => "overridden shared event !!!")
                .Build();

            var events = new[] { Event.E1, Event.E2, Event.E0, Event.E1, Event.E3, Event.E0 };

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
