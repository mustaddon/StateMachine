using FluentStateMachine;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Example.ConsoleApp;

// define IStateMachine factory for specific requests
public class ExampleFactory : IFsmFactory<ExampleFactoryRequest>
{
    public async Task<IStateMachine> Create(ExampleFactoryRequest request, CancellationToken cancellationToken = default)
    {
        // get entity from storage
        var entity = await MockEntityStore.GetEntityById(request.MyEntityId);

        // build IStateMachine for the entity
        return new FsmBuilder<States, Type>(entity.State)
            .OnEnter(x =>
            {
                Console.WriteLine($"{x.Event.Name}: Enter state '{x.State}' from '{x.PrevState}'");
                // update entity state
                entity.State = x.State;
            })
            .On<MediatorRequest0, string>().Execute(x => "shared to all states test event")

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
                .On<MediatorRequest0, string>()
                    .Execute(x => $"overridden shared result")

            .Build(concurrent: true); // test concurrent StateMachine
    }
}