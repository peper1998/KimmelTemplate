using KimmelTemplate.PublishedLanguage.Commands;
using KimmelTemplate.PublishedLanguage.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KimmelTemplate.Api.Controllers
{
    [Route("todos")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TodosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetTodos([FromQuery] GetTodosQuery query)
        {
            await _mediator.Send(query);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddMessage([FromBody] CreateTodoCommand command)
        {
            await _mediator.Send(command);
            return Ok();
        }
    }
}
