using MediatR;
using ProductService.Domain.Dtos;
public record UpdateProductCommand : ProductDto, IRequest<ProductDto>
{
    protected UpdateProductCommand(ProductDto original) : base(original)
    {
    }
}
