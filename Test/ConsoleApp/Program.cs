using RandomSolutions;
using System;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        enum State { S1, S2, S3 }
        enum Event { E1, E2, E3 }

        static IStateMachine<State, Event> CreateFsm()
        {
            return new FsmBuilder<State, Event>(State.S1)
                .OnJump(x => Console.WriteLine($"On jump to {x.Fsm.Current} from {x.PrevState}"))
                .OnReset(x => Console.WriteLine($"On reset to {x.Fsm.Current} from {x.PrevState}"))
                .OnTrigger(x => Console.WriteLine($"On trigger {x.Event}"))
                .OnError(x => Console.WriteLine($"On error {x.Fsm.Current}: {x.Message}"))
                .OnEnter(x => Console.WriteLine($"Enter state {x.Fsm.Current} from {x.PrevState}"))
                .OnExit(x => Console.WriteLine($"Exit state {x.Fsm.Current} to {x.NextState}"))

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
                .Build();
        }

        static async Task Main(string[] args)
        {
            var fsm = CreateFsm();
            var events = new[] { Event.E1, Event.E2, Event.E1, Event.E3 };

            foreach (var e in events)
            {
                Console.WriteLine($"{fsm.Current}: {string.Join(", ", fsm.GetEvents())}");
                Console.WriteLine($"Result: {fsm.Trigger(e)}\n");
            }

            fsm.Reset();

            Console.WriteLine($"\n\n=== ASYNC ===\n");

            foreach (var e in events)
            {
                Console.WriteLine($"{fsm.Current}: {string.Join(", ", await fsm.GetEventsAsync())}");
                Console.WriteLine($"Result: {await fsm.TriggerAsync(e)}\n");
            }

            await fsm.ResetAsync();
        }
    }
}
