using KimmelTemplate.Common.CQRS;
using KimmelTemplate.PublishedLanguage.Dtos;

namespace KimmelTemplate.PublishedLanguage.Queries
{
    public class GetTodosQuery : IQuery<IEnumerable<TodoDTO>>
    {
    }
}
