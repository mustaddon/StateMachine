using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FluentStateMachine.MediatR;

public class FsmPipelineBehavior<TRequest, TResponse>(IServiceProvider serviceProvider)
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is IFsmRequestBase)
        {
            var fsm = (IEventController<Type>)await serviceProvider
                .GetRequiredService<IFsmFactory<TRequest>>()
                .Create(request, cancellationToken);

            return await fsm.TriggerAsync<TResponse>(request, cancellationToken);
        }

        return await next();
    }
}
