using MediatR;

namespace FluentStateMachine.MediatR;

public interface IFsmRequestBase : IBaseRequest, IFsmEventBase;
public interface IFsmRequest : IFsmRequestBase, IRequest, IFsmEvent;
public interface IFsmRequest<TResult> : IFsmRequestBase, IRequest<TResult>, IFsmEvent<TResult>;
