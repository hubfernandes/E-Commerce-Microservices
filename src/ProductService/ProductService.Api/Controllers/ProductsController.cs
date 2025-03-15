using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Commands;
using ProductService.Application.Queries;

namespace ProductService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery(id));
            return product == null ? NotFound() : Ok(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _mediator.Send(new GetAllProductsQuery());
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            var product = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand command)
        {
            var product = await _mediator.Send(command);
            return product == null ? NotFound() : Ok(product);
        }

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteProduct(int id)
        //{
        //    var result = await _mediator.Send(new DeleteProductCommand(id)); // Add this command if needed
        //    return result ? NoContent() : NotFound();
        //}
    }
}
