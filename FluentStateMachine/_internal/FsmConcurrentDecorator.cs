﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FluentStateMachine._internal;


internal class FsmConcurrentDecorator<TState, TEvent>(IStateMachine<TState, TEvent> fsm) : IConcurrentStateMachine<TState, TEvent> 
{
    readonly IStateMachine<TState, TEvent> _fsm = fsm;
    readonly SemaphoreSlim _semaphore = new(1, 1);

    public TState Current => _fsm.Current;
    public ICollection<TState> States => _fsm.States;
    public ICollection<TEvent> Events => _fsm.Events;

    public Task<bool> IsAvailableEventAsync(TEvent value, object data = null, CancellationToken cancellationToken = default)
        => _fsm.IsAvailableEventAsync(value, data, cancellationToken);

    public Task<bool> IsAvailableStateAsync(TState value, object data = null, CancellationToken cancellationToken = default)
        => _fsm.IsAvailableStateAsync(value, data, cancellationToken);

    public async Task<TResult> TriggerAsync<TResult>(TEvent e, object data = null, CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try { return await _fsm.TriggerAsync<TResult>(e, data, cancellationToken); }
        finally { _semaphore.Release(); }
    }

    public async Task<TResult> TriggerIfAvailableAsync<TResult>(TEvent e, object data = null, CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try { return await _fsm.IsAvailableEventAsync(e, data, cancellationToken) ? await _fsm.TriggerAsync<TResult>(e, data, cancellationToken) : default; }
        finally { _semaphore.Release(); }
    }

    public async Task JumpToAsync(TState state, object data = null, CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try { await _fsm.JumpToAsync(state, data, cancellationToken); }
        finally { _semaphore.Release(); }
    }

    public async Task<bool> TryJumpToAsync(TState state, object data = null, CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (!await _fsm.IsAvailableStateAsync(state, data, cancellationToken))
                return false;

            await _fsm.JumpToAsync(state, data, cancellationToken);
            return true;
        }
        finally { _semaphore.Release(); }
    }

    public async Task ResetToAsync(TState state, CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try { await _fsm.ResetToAsync(state, cancellationToken); }
        finally { _semaphore.Release(); }
    }

    public async Task ResetAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try { await _fsm.ResetAsync(cancellationToken); }
        finally { _semaphore.Release(); }
    }
}


interface IConcurrentStateMachine<TState, TEvent> : IStateMachine<TState, TEvent>, IConcurrentStateController<TState>, IConcurrentEventController<TEvent>;

interface IConcurrentStateController<TState> : IStateController<TState>
{
    Task<bool> TryJumpToAsync(TState state, object data = null, CancellationToken cancellationToken = default);
}
interface IConcurrentEventController<TEvent> : IEventController<TEvent>
{
    Task<TResult> TriggerIfAvailableAsync<TResult>(TEvent e, object data = null, CancellationToken cancellationToken = default);
}