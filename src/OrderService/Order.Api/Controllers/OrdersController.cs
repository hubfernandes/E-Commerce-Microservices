using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Commands;
using Order.Application.Queries;

namespace Order.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(CreateOrder), new { id = result }, new { Id = result });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch between route and command");

            var result = await _mediator.Send(command);
            return (bool)result.Succeeded! ? Ok(result) : BadRequest(result);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var command = new DeleteOrderCommand(id);
            var result = await _mediator.Send(command);
            return (bool)result.Succeeded! ? Ok(result) : BadRequest(result);

        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var query = new GetAllOrdersQuery();
            var result = await _mediator.Send(query);
            return (bool)result.Succeeded! ? Ok(result) : NotFound(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var query = new GetOrderByIdQuery(id);
            var result = await _mediator.Send(query);
            return (bool)result.Succeeded! ? Ok(result) : NotFound(result);
        }
    }
}
