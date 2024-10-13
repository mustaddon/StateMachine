using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FluentStateMachine._internal;

internal class FsmFactoryRequestAdapter<TRequest>(
    IServiceProvider serviceProvider,
    FsmFactoryRegister registredFactories)
    : IFsmFactory<TRequest>
{
    public Task<IStateMachine> Create(TRequest request, CancellationToken cancellationToken)
    {
        var factoryType = registredFactories.FindFactoryTypeForRequest(typeof(TRequest))
            ?? throw new InvalidOperationException($"No IFsmFactory for type '{typeof(TRequest)}' has been registered.");

        return ((IFsmFactory<TRequest>)serviceProvider.GetRequiredService(factoryType))
            .Create(request, cancellationToken);
    }
}
