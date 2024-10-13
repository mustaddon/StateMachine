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
            // REQUIRED: register FSM services
            .AddFsm(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
            .AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                // REQUIRED: add FSM behavior
                cfg.AddFsmBehavior();
            })
            .BuildServiceProvider();

        var mediator = services.GetRequiredService<MediatR.IMediator>();

        var exampleRequests = new ExampleFactoryRequest[] {
            new MediatorRequest1 { MyEntityId = 7, Num = 111 },
            new MediatorRequest2 { MyEntityId = 7, Bit = true },
            new MediatorRequest0 { MyEntityId = 7, },
            new MediatorRequest1 { MyEntityId = 7, Num = 555 },
            new MediatorRequest3 { MyEntityId = 7, Num = 777, Text = "test" },
            new MediatorRequest0 { MyEntityId = 7, },
        };

        foreach (var request in exampleRequests)
        {
            Console.WriteLine($">>> Sending request '{request.GetType()}'");

            // The request starts FSM, which is defined in MediatorExample.StateMachines.cs
            var result = await mediator.Send(request);

            Console.WriteLine($"Result: {result}\n");
        }

        Console.WriteLine($"Done\n");
    }
}


