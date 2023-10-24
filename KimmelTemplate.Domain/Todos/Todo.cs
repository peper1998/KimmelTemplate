namespace KimmelTemplate.Domain.Todos
{
    public class Todo
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        protected Todo()
        { }

        public Todo(Guid id, string title)
        {
            Id = id;
            Title = title;
        }
    }
}
