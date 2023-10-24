using MediatR;

namespace KimmelTemplate.Common.CQRS
{
    public interface IQuery<TResult> : IRequest<TResult>
    { }
}
