using InventoryService.Application.Commands;
using InventoryService.Application.Queries;
using InventoryService.Domain.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController(IMediator _mediator) : ControllerBase
    {


        [HttpGet("{productId}")]
        public async Task<ActionResult<StockDto>> GetStock(int productId)
        {
            var query = new GetStockByProductIdQuery(productId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("add-stock")]
        public async Task<IActionResult> AddStock([FromBody] AddStockCommand command)
        {
            var result = await _mediator.Send(command);
            return (bool)(result.Succeeded!) ? Ok(result) : BadRequest(result);
        }


        [HttpPost("reserve")]
        public async Task<IActionResult> ReserveStock([FromBody] ReserveStockCommand command)
        {
            var result = await _mediator.Send(command);
            return (bool)(result.Succeeded!) ? Ok(result) : BadRequest(result);
        }

        [HttpPost("release")]
        public async Task<IActionResult> ReleaseStock([FromBody] ReleaseStockCommand command)
        {
            var result = await _mediator.Send(command);
            return (bool)(result.Succeeded!) ? Ok(result) : BadRequest(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateStock([FromBody] UpdateStockCommand command)
        {
            var result = await _mediator.Send(command);
            return (bool)(result.Succeeded!) ? Ok(result) : BadRequest(result);
        }

        [HttpGet("low-stock")]
        public async Task<ActionResult<List<LowStockDto>>> GetLowStock([FromQuery] int threshold)
        {
            var query = new GetLowStockQuery(threshold);
            var result = await _mediator.Send(query);
            return (bool)(result.Succeeded!) ? Ok(result) : BadRequest(result);
        }

        [HttpPost("reconcile")]
        public async Task<IActionResult> ReconcileStock([FromBody] ReconcileStockCommand command)
        {
            var result = await _mediator.Send(command);
            return (bool)(result.Succeeded!) ? Ok(result) : BadRequest(result);
        }
    }
}
