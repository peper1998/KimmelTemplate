using KimmelTemplate.Common.CQRS;

namespace KimmelTemplate.PublishedLanguage.Commands
{
    public class CreateTodoCommand : ICommand
    {
        public string Title { get; set; }
    }
}
