using FluentStateMachine.MediatR;

namespace Microsoft.Extensions.DependencyInjection;

public static class FsmMediatRConfigurationExtensions
{
    public static MediatRServiceConfiguration AddFsmBehavior(this MediatRServiceConfiguration configuration)
    {
        configuration.AddOpenBehavior(typeof(FsmPipelineBehavior<,>));

        return configuration;
    }
}
