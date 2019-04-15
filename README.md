# StateMachine
[![NuGet version](https://badge.fury.io/nu/RandomSolutions.StateMachine.svg)](http://badge.fury.io/nu/RandomSolutions.StateMachine)

## Example
```C#
enum State { S1, S2, S3 }
enum Event { E1, E2, E3 }
```

```C#
var fsm = new FsmBuilder<State, Event>(State.S1)
    .OnJump(x => Console.WriteLine($"State change to {x.Fsm.Current} from {x.PrevState}"))    
    .State(State.S1)
        .On(Event.E1).Execute(x => { /* some operations */ return "some data"; })
        .On(Event.E2).JumpTo(State.S2)
    .State(State.S2)
        .On(Event.E3).Enable(x => /* some conditions */ true).JumpTo(State.S3)
    .State(State.S3)
        .OnEnter(x => Console.WriteLine($"Enter to final state"))
    .Build();


fsm.Trigger(Event.E2);
fsm.Trigger(Event.E3);


// Console output:
// State change to S2 from S1
// State change to S3 from S2
// Enter to final state
```

[More details in the test console application...](Test/ConsoleApp/Program.cs)
