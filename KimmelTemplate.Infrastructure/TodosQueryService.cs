using KimmelTemplate.ApplicationServices.Boundaries;
using KimmelTemplate.Infrastructure.DataModel.Context;
using KimmelTemplate.PublishedLanguage.Dtos;
using KimmelTemplate.PublishedLanguage.Queries;

namespace KimmelTemplate.Infrastructure
{
    public class TodosQueryService : ITodosQueryService
    {
        private readonly TodosContext _context;

        public TodosQueryService(TodosContext context)
        {
            _context = context;
        }

        public async Task<IList<TodoDTO>> GetTodos(GetTodosQuery query)
        {
            var todosQuery = _context.Todos.AsQueryable();

            return await todosQuery.Select(todo => new TodoDTO()
            {
                Id = todo.Id,
                Title = todo.Title,
            }).ToListAsync();
        }
    }
}
