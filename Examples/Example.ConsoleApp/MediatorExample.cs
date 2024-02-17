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
        
        var requests = new object[] {
            new MediatorRequest1 { Num = 111 },
            new MediatorRequest2 { Bit = true },
            new MediatorRequest0 { },
            new MediatorRequest1 { Num = 555 },
            new MediatorRequest3 { Num = 777, Text = "test" },
            new MediatorRequest0 { }
        };

        foreach (var request in requests)
        {
            var result = await mediator.Send(request);
            Console.WriteLine($"Result: {result}\n\n");
        }

        Console.WriteLine($"Done\n");
    }

    public class StateMachineBuilder
    {
        public static IStateMachine<States, Type> Build()
        {
            return new FsmBuilder<States, Type>(States.S1)
                .OnError(x => Console.WriteLine($"{x.Event.Name}: On error ({x.Error})"))
                .OnTrigger(x => Console.WriteLine($"{x.Event.Name}: Triggered in state '{x.State}'"))
                .OnComplete(x => Console.WriteLine($"{x.Event.Name}: On complete (triggered and state{(x.State == x.PrevState ? " NOT " : " ")}changed)"))
                .OnExit(x => Console.WriteLine($"{x.Event.Name}: Exit state '{x.State}' to '{x.NextState}'"))
                .OnEnter(x => Console.WriteLine($"{x.Event.Name}: Enter state '{x.State}' from '{x.PrevState}'"))
                .OnJump(x => Console.WriteLine($"{x.Event.Name}: On jump to '{x.State}' from '{x.PrevState}'"))
                .OnReset(x => Console.WriteLine($"Reset to '{x.State}' from '{x.PrevState}'"))
                .On<MediatorRequest0>().Execute(x => "shared to all states")

                .State(States.S1)
                    .On<MediatorRequest1, double>()
                        .Execute(x =>
                        {
                            Console.WriteLine($"{x.Event.Name}: Execute with args cast and results type check");
                            return x.Data.Num / 10d; // some result
                        })
                    .On<MediatorRequest2>()
                        .JumpTo(x => x.Data.Bit ? States.S2 : States.S3) // condition jump
                    .On<MediatorRequest3, string>()
                        .Execute(x => $"({x.Data.Num}, {x.Data.Text})")
                        .JumpTo(States.S3)

                .State(States.S2)
                    //.OnEnter(x => x.Fsm.JumpTo(States.S3)) // test skip state
                    .Enable(async x => { await Task.Delay(500); return true; })
                    .On<MediatorRequest1, double>()
                        .Execute(x => x.Data.Num / 100d)
                        .JumpTo(async x => { await Task.Delay(500); return States.S1; })

                .State(States.S3)
                    .OnEnter(x => Console.WriteLine($"{x.Event.Name}: Final state !!!"))
                    .On<MediatorRequest0>()
                        .Execute(x => $"overridden shared result")

                .Build(concurrent: true);
        }
    }
}


