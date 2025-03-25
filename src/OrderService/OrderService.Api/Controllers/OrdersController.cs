using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands;
using OrderService.Application.Queries;

namespace OrderService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(IMediator _mediator) : ControllerBase
    {


        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return (bool)result.Succeeded! ? CreatedAtAction(nameof(CreateOrder), result) : BadRequest(result);
        }

        [HttpPost("order-from_cart")]
        public async Task<IActionResult> CreateOrderFromCart([FromBody] CreateOrderFromCartCommand command)
        {
            var result = await _mediator.Send(command);
            return (bool)result.Succeeded! ? CreatedAtAction(nameof(CreateOrder), result) : BadRequest(result);
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
