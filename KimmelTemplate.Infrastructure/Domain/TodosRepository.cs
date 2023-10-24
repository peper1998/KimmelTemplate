using KimmelTemplate.ApplicationServices.Boundaries;
using KimmelTemplate.Domain.Todos;
using KimmelTemplate.Infrastructure.DataModel.Context;

namespace KimmelTemplate.Infrastructure.Domain
{
    public class TodosRepository : ITodosRepository
    {
        private readonly TodosContext _context;

        public TodosRepository(TodosContext context)
        {
            _context = context;
        }

        public void Store(Todo todo)
        {
            _context.Todos.Add(todo);
        }
    }
}
