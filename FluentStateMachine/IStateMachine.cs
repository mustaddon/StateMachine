﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FluentStateMachine;

public interface IStateMachine<TState, TEvent> : IStateMachine, IStateController<TState>, IEventController<TEvent>;
public interface IStateMachine : IStateController, IEventController;



public interface IStateController<TState> : IStateController
{
    TState Current { get; }
    ICollection<TState> States { get; }
    Task<bool> IsAvailableStateAsync(TState value, object data = null, CancellationToken cancellationToken = default);
    Task JumpToAsync(TState state, object data = null, CancellationToken cancellationToken = default);
    Task ResetToAsync(TState state, CancellationToken cancellationToken = default);
}
public interface IStateController
{
    Task ResetAsync(CancellationToken cancellationToken = default);
}



public interface IEventController<TEvent> : IEventController
{
    ICollection<TEvent> Events { get; }
    Task<bool> IsAvailableEventAsync(TEvent value, object data = null, CancellationToken cancellationToken = default);
    Task<TResult> TriggerAsync<TResult>(TEvent e, object data = null, CancellationToken cancellationToken = default);
}
public interface IEventController;