﻿using FluentStateMachine;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Example.ConsoleApp;

public class MediatorHandler<T, TResult>(IStateMachine<States, Type> fsm) : IRequestHandler<T, TResult>
    where T : IFsmEvent<TResult>, IRequest<TResult>
{
    public Task<TResult> Handle(T request, CancellationToken cancellationToken)
    {
        return fsm.TriggerAsync<TResult>(typeof(T), request, cancellationToken);
    }
}

public class MediatorHandler<T>(IStateMachine<States, Type> fsm) : IRequestHandler<T>
    where T : IFsmEvent, IRequest
{
    public Task Handle(T request, CancellationToken cancellationToken)
    {
        return fsm.TriggerAsync(typeof(T), request, cancellationToken);
    }
}
