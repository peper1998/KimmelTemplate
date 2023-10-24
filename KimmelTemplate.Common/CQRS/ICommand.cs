using MediatR;

namespace KimmelTemplate.Common.CQRS
{
    public interface IWriteRequest
    {

    }

    public interface ICommand : IWriteRequest, IRequest, IRequest<Unit>
    {

    }

    public interface ICommand<out TResponse> : IWriteRequest, IRequest<TResponse>
    {

    }
}
