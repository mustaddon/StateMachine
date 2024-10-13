using MediatR;

namespace FluentStateMachine.MediatR;

public interface IFsmRequestBase : IBaseRequest;
public interface IFsmRequest : IFsmRequestBase, IRequest, IFsmEvent;
public interface IFsmRequest<TResult> : IFsmRequestBase, IRequest<TResult>, IFsmEvent<TResult>;
