using MediatR;
using ProductService.Domain.Dtos;

namespace ProductService.Application.Commands
{
    public record CreateProductCommand : ProductDto, IRequest<ProductDto>
    {
        protected CreateProductCommand(ProductDto original) : base(original)
        {
        }
    }
}