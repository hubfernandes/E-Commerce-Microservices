using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Commands;
using ProductService.Application.Queries;

namespace ProductService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[AllowAnonymous]
    public class ProductsController(IMediator _mediator) : ControllerBase
    {

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var result = await _mediator.Send(new GetProductByIdQuery(id));
            return (bool)result.Succeeded! ? Ok(result) : NotFound(result);
        }
        // [Authorize(Roles = "user")]
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _mediator.Send(new GetAllProductsQuery());
            return (bool)result.Succeeded! ? Ok(result) : NotFound(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string query)
        {
            var result = await _mediator.Send(new SearchProductsQuery(query));
            return (bool)result.Succeeded! ? Ok(result) : NotFound(result);
        }

        [HttpGet("{id}/stock")]
        public async Task<IActionResult> GetProductStock(int id)
        {
            var result = await _mediator.Send(new GetProductStockQuery(id));
            return (bool)result.Succeeded! ? Ok(result) : NotFound(result);
        }

        //   [Authorize(Roles = "admin")]
        [HttpGet("low-stock/{threshold}")]
        public async Task<IActionResult> GetLowStockProducts(int threshold = 10)
        {
            var result = await _mediator.Send(new GetLowStockProductsQuery(threshold));
            return (bool)result.Succeeded! ? Ok(result) : NotFound(result);
        }

        //    [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            var result = await _mediator.Send(command);
            return (bool)result.Succeeded! ? CreatedAtAction(nameof(CreateProduct), result) : NotFound(result);

        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand command)
        {
            var result = await _mediator.Send(command);
            return (bool)result.Succeeded! ? Ok(result) : NotFound(result);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _mediator.Send(new DeleteProductCommand(id));
            return (bool)result.Succeeded! ? Ok(result) : NotFound(result);
        }
    }
}
