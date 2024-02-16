using FluentStateMachine;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Example.ConsoleApp;

internal class MediatorExample
{
    public static async Task Run()
    {
        Console.WriteLine($"=== MediatorExample Start ===\n");

        var services = new ServiceCollection()
            .AddSingleton(x => StateMachineBuilder.Build())
            .AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
            .AddTransient(typeof(IRequestHandler<,>), typeof(MediatorHandler<,>))
            .BuildServiceProvider();

        var mediator = services.GetRequiredService<IMediator>();
        var fsm = services.GetRequiredService<IStateMachine<States, IMediatorEvent>>();

        var testEvents = new IMediatorEvent[] {
            new MediatorEvent1 { Num = 10 },
            new MediatorEvent2 { Bit = true },
            new MediatorEvent0 { Text = "test A" },
            new MediatorEvent1 { Num = 555 },
            new MediatorEvent3 { Num = 777, Text = "test B" },
            new MediatorEvent0 { Text = "text C" }
        };

        foreach (var e in testEvents)
        {
            Console.WriteLine($"Current state: {fsm.Current}");
            Console.WriteLine($"Available events: {string.Join(", ", await fsm.GetAvailableEventsAsync(e))}");
            Console.WriteLine($"Result: {await mediator.Send(e)}\n\n");
        }

        Console.WriteLine($"Done\n");
    }

    public class StateMachineBuilder
    {
        public static IStateMachine<States, IMediatorEvent> Build()
        {
            return new FsmBuilder<States, IMediatorEvent>(States.S1)
               .OnError(x => Console.WriteLine($"{x.Event}: On error ({x.Error})"))
               .OnTrigger(x => Console.WriteLine($"{x.Event}: On trigger"))
               .OnComplete(x => Console.WriteLine($"{x.Event}: On complete (triggered and state{(x.State == x.PrevState ? " NOT " : " ")}changed)"))
               .OnExit(x => Console.WriteLine($"{x.Event}: Exit state {x.State} to {x.NextState}"))
               .OnEnter(x => Console.WriteLine($"{x.Event}: Enter state {x.State} from {x.PrevState}"))
               .OnJump(x => Console.WriteLine($"{x.Event}: On jump to {x.State} from {x.PrevState}"))
               .OnReset(x => Console.WriteLine($"Reset to {x.State} from {x.PrevState}"))
               .OnX(MediatorEvent0.Value).Execute(x => "shared to all states")

               .State(States.S1)
                   .OnX(MediatorEvent1.Value)
                       .Execute(x =>
                       {
                           Console.WriteLine($"{x.Event}: Execute with args cast and results type check");
                           return x.Data.Num * 10; // some result
                       })
                   .OnX(MediatorEvent2.Value).JumpTo(States.S2)
                   .OnX(MediatorEvent3.Value).Execute(x => x.Data.ToString()).JumpTo(States.S3)
               .State(States.S2)
                   //.OnEnter(x => x.Fsm.JumpTo(States.S3)) // test skip state
                   .Enable(async x => { await Task.Delay(500); return true; })
                   .OnX(MediatorEvent1.Value)
                       .Execute(x => x.Data.Num * 100)
                       .JumpTo(async x => { await Task.Delay(500); return States.S1; })
               .State(States.S3)
                   .OnEnter(x => Console.WriteLine($"{x.Event}: Final state !!!"))
                   .OnX(MediatorEvent0.Value).Execute(x => $"overridden shared result !!!")
               .Build(concurrent: true);
        }
    }
}


