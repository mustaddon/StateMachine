# FluentStateMachine.MediatR [![NuGet version](https://badge.fury.io/nu/FluentStateMachine.MediatR.svg)](http://badge.fury.io/nu/FluentStateMachine.MediatR)
Finite-state machine (FSM) with a fluent interface and MediatR compatibility

## Example
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

var response = await mediator.Send(new ExampleMediatorRequest { MyEntityId = 7 });
```
[The full code example can be found here...](https://github.com/mustaddon/StateMachine/blob/master/Examples/Example.ConsoleApp/MediatorExample.cs)
