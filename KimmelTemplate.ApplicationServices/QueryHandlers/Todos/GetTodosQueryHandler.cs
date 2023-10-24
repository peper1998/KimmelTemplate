using KimmelTemplate.PublishedLanguage.Dtos;
using KimmelTemplate.PublishedLanguage.Queries;
using MediatR;

namespace KimmelTemplate.ApplicationServices.QueryHandlers.Todos
{
    public class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, IEnumerable<TodoDTO>>
    {
        public Task<IEnumerable<TodoDTO>> Handle(GetTodosQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
