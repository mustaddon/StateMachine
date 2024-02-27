# FluentStateMachine [![NuGet version](https://badge.fury.io/nu/FluentStateMachine.svg)](http://badge.fury.io/nu/FluentStateMachine)
Finite-state machine (FSM) with a fluent interface and mediators compatibility

## Example
```C#
enum States { S1, S2, S3 }
enum Events { E1, E2, E3 }
```
```C#
var fsm = new FsmBuilder<States, Events>(States.S1)
    .OnEnter(x => Console.WriteLine($"State change to {x.State} from {x.PrevState}"))
    .State(States.S1)
        .On(Events.E1).Execute(x => { /* some operations */ return "some results"; })
        .On(Events.E2).JumpTo(States.S2)
    .State(States.S2)
        .On(Events.E3).Enable(x => /* some conditions */ true).JumpTo(States.S3)
    .State(States.S3)
        .OnEnter(x => Console.WriteLine($"Enter to final state"))
    .Build();


Console.WriteLine(fsm.Trigger(Events.E1));
fsm.Trigger(Event.E2);
fsm.Trigger(Event.E3);


// Console output:
// some results
// State change to S2 from S1
// State change to S3 from S2
// Enter to final state
```

[ReadmeExample1.cs](https://github.com/mustaddon/StateMachine/blob/master/Examples/Example.ConsoleApp/ReadmeExample1.cs)


## Example with type-based events
```C#
class Event1 : IFsmEvent<string>
{
    public string? SomeProp { get; set; }
}

class Event2 : IFsmEvent<object>;
class Event3 : IFsmEvent<object>;
```
```C#
var fsm = new FsmBuilder<States, Type>(States.S1)
    .OnEnter(x => Console.WriteLine($"State change to {x.State} from {x.PrevState}"))
    .State(States.S1)
        .On<Event1, string>().Execute(x => { /* some operations */ return $"{x.Data.SomeProp} results"; })
        .On<Event2>().JumpTo(States.S2)
    .State(States.S2)
        .On<Event3>().Enable(x => /* some conditions */ true).JumpTo(States.S3)
    .State(States.S3)
        .OnEnter(x => Console.WriteLine($"Enter to final state"))
    .Build();


Console.WriteLine(fsm.Trigger(new Event1
{
    SomeProp = "example"
}));

fsm.Trigger(new Event2());
fsm.Trigger(new Event3());


// Console output:
// example results
// State change to S2 from S1
// State change to S3 from S2
// Enter to final state
```
[ReadmeExample2.cs](https://github.com/mustaddon/StateMachine/blob/master/Examples/Example.ConsoleApp/ReadmeExample2.cs)

[Example of use for workflows](https://github.com/mustaddon/StateMachine/blob/master/Examples/Example.ConsoleApp/WorkflowExample.cs)

[Example with mediator (MediatR)](https://github.com/mustaddon/StateMachine/blob/master/Examples/Example.ConsoleApp/MediatorExample.cs)
