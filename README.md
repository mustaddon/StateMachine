# FluentStateMachine [![NuGet version](https://badge.fury.io/nu/FluentStateMachine.svg)](http://badge.fury.io/nu/FluentStateMachine)
Finite-state machine (FSM) with a fluent interface and mediators compatibility

## Example
```C#
enum States { S1, S2, S3 }
enum Events { E1, E2, E3 }
```

```C#
var fsm = new FsmBuilder<States, Events>(States.S1)
    .OnEnter(x => Console.WriteLine($"State change to {x.Fsm.Current} from {x.PrevState}"))
    .State(States.S1)
        .On(Events.E1).Execute(x => { /* some operations */ return "some data"; })
        .On(Events.E2).JumpTo(States.S2)
    .State(States.S2)
        .On(Events.E3).Enable(x => /* some conditions */ true).JumpTo(States.S3)
    .State(States.S3)
        .OnEnter(x => Console.WriteLine($"Enter to final state"))
    .Build();


fsm.Trigger(Event.E2);
fsm.Trigger(Event.E3);


// Console output:
// State change to S2 from S1
// State change to S3 from S2
// Enter to final state
```

[Example.ConsoleApp](https://github.com/mustaddon/StateMachine/blob/master/Examples/Example.ConsoleApp/Program.cs)
