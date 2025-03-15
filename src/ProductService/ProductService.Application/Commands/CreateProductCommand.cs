using MediatR;
using ProductService.Domain.Dtos;
using Shared.Bases;

namespace ProductService.Application.Commands
{
    public record CreateProductCommand(int Id, string Name, decimal Price, int Stock, bool IsActive)
     : ProductDto(Id, Name, Price, Stock, IsActive), IRequest<Response<string>>;
}