using System.Threading;
using System.Threading.Tasks;

namespace FluentStateMachine;


public interface IFsmFactory<in TRequest>
{
    Task<IStateMachine> Create(TRequest request, CancellationToken cancellationToken = default);
}