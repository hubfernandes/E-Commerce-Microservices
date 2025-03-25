using CartService.Application.Commands;
using CartService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CartService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CartsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCartById(int id)
        {
            var query = new GetCartByIdQuery(id);
            var result = await _mediator.Send(query);
            return (bool)result.Succeeded! ? Ok(result) : NotFound(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetCartsByUserId(string userId)
        {
            var result = await _mediator.Send(new GetCartsByUserIdQuery(userId));
            return (bool)result.Succeeded! ? Ok(result) : NotFound(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllCarts()
        {
            var query = new GetAllCartsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCart([FromBody] CreateCartCommand command)
        {
            var result = await _mediator.Send(command);
            return (bool)result.Succeeded! ? CreatedAtAction(nameof(CreateCart), result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCart(int id, [FromBody] UpdateCartCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch between route and command");

            var result = await _mediator.Send(command);
            return (bool)result.Succeeded! ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            var command = new DeleteCartCommand(id);
            var result = await _mediator.Send(command);
            return (bool)result.Succeeded! ? Ok(result) : NotFound(result);
        }

        [HttpDelete("delete-by-UserId/{userId}")]
        public async Task<IActionResult> DeleteCartByUserId(string userId)
        {
            var command = new DeleteCartByUserIdCommand(userId);
            var result = await _mediator.Send(command);
            return (bool)result.Succeeded! ? Ok(result) : NotFound(result);
        }


    }
}