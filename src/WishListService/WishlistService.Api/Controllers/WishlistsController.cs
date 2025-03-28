using MediatR;
using Microsoft.AspNetCore.Mvc;
using WishlistService.Application.Commands;
using WishlistService.Application.Queries;

namespace WishlistService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WishlistsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WishlistsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWishlistById(int id)
        {
            var result = await _mediator.Send(new GetWishlistByIdQuery(id));
            return (bool)result.Succeeded! ? Ok(result) : NotFound(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetWishlistByUserId(string userId)
        {
            var result = await _mediator.Send(new GetWishlistByUserIdQuery(userId));
            return (bool)result.Succeeded! ? Ok(result) : NotFound(result);

        }

        [HttpGet]
        public async Task<IActionResult> GetAllWishlists()
        {
            var result = await _mediator.Send(new GetAllWishlistsQuery());
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWishlist([FromBody] CreateWishlistCommand command)
        {
            var result = await _mediator.Send(command);
            return (bool)result.Succeeded! ? CreatedAtAction(nameof(GetWishlistById), new { id = result.Data }, result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWishlist(int id, [FromBody] UpdateWishlistCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch");

            var result = await _mediator.Send(command);
            return (bool)result.Succeeded! ? Ok(result) : BadRequest(result);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWishlist(int id)
        {
            var result = await _mediator.Send(new DeleteWishlistCommand(id));
            return (bool)result.Succeeded! ? Ok(result) : NotFound(result);

        }

        [HttpDelete("user/{userId}")]
        public async Task<IActionResult> DeleteWishlistByUserId(string userId)
        {
            var result = await _mediator.Send(new DeleteWishlistByUserIdCommand(userId));
            return (bool)result.Succeeded! ? Ok(result) : NotFound(result);

        }

        [HttpPost("add-item")]
        public async Task<IActionResult> AddItemToWishlist([FromBody] AddItemToWishlistCommand command)
        {
            var result = await _mediator.Send(command);
            return (bool)result.Succeeded! ? Ok(result) : BadRequest(result);

        }

        [HttpPost("remove-item")]
        public async Task<IActionResult> RemoveItemFromWishlist([FromBody] RemoveItemFromWishlistCommand command)
        {
            var result = await _mediator.Send(command);
            return (bool)result.Succeeded! ? Ok(result) : BadRequest(result);

        }
    }
}