using KimmelTemplate.PublishedLanguage.Dtos;
using KimmelTemplate.PublishedLanguage.Queries;

namespace KimmelTemplate.ApplicationServices.Boundaries
{
    //not very proud of the name
    public interface ITodosQueryService
    {
        Task<IList<TodoDTO>> GetTodos(GetTodosQuery query);
    }
}
