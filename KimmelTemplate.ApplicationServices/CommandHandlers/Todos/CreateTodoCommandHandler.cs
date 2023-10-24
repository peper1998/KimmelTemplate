using KimmelTemplate.ApplicationServices.Boundaries;
using KimmelTemplate.Domain;
using KimmelTemplate.Domain.Todos;
using KimmelTemplate.PublishedLanguage.Commands;
using MediatR;

namespace KimmelTemplate.ApplicationServices.CommandHandlers.Todos
{
    public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand>
    {
        private readonly ITodosRepository _todosRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateTodoCommandHandler(ITodosRepository todosRepository, IUnitOfWork unitOfWork)
        {
            _todosRepository = todosRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CreateTodoCommand request, CancellationToken cancellationToken)
        {
            _todosRepository.Store(new Todo(Guid.NewGuid(), request.Title));
            await _unitOfWork.Save();
        }
    }
}
