using FluentStateMachine;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Example.ConsoleApp;

public class MediatorHandler<T, TResult>(IStateMachine<States, IMediatorEvent> fsm) : IRequestHandler<T, TResult>
    where T : IMediatorEvent<IMediatorEvent, TResult>
{
    public Task<TResult> Handle(T request, CancellationToken cancellationToken)
    {
        return fsm.TriggerAsyncX(request, cancellationToken);
    }
}
