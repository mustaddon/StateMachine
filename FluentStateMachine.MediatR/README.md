# FluentStateMachine.MediatR [![NuGet version](https://badge.fury.io/nu/FluentStateMachine.MediatR.svg)](http://badge.fury.io/nu/FluentStateMachine.MediatR)
Finite-state machine (FSM) with a fluent interface and MediatR compatibility.

<!-- ![](https://raw.githubusercontent.com/mustaddon/StateMachine/master/FluentStateMachine.MediatR/dgrm_small.png) -->
<img src="https://raw.githubusercontent.com/mustaddon/StateMachine/master/FluentStateMachine.MediatR/dgrm.png" width="400" />

### Example
```C#
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

var response = await mediator.Send(new MediatorRequest1 { MyEntityId = 7 });
```

### Define IStateMachine factory
```C#
public class ExampleFactory : IFsmFactory<ExampleFactoryRequest>
{
    public async Task<IStateMachine> Create(ExampleFactoryRequest request, CancellationToken cancellationToken = default)
    {
        // get entity from storage
        var entity = await MockEntityStore.GetEntityById(request.MyEntityId);

        // build IStateMachine for the entity
        return new FsmBuilder<States, Type>(entity.State)
            // update entity state on change
            .OnEnter(x => entity.State = x.State) 

            .State(States.S1)
                .On<MediatorRequest1>().JumpTo(States.S2)

            .State(States.S2)
                .OnEnter(x => Console.WriteLine($"{x.Event.Name}: Final state !!!"))
                .On<MediatorRequest2, string>().Execute(x => "some result")

            .Build();
    }
}
```
[The full code example can be found here...](https://github.com/mustaddon/StateMachine/blob/master/Examples/Example.ConsoleApp/MediatorExample.cs)
