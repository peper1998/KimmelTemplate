using KimmelTemplate.Domain.Todos;

namespace KimmelTemplate.ApplicationServices.Boundaries
{
    public interface ITodosRepository
    {
        void Store(Todo todo);
    }
}
