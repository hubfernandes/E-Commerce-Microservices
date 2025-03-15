using MediatR;
using ProductService.Domain.Dtos;
using Shared.Bases;
public record UpdateProductCommand(int Id, string Name, decimal Price, int Stock, bool IsActive)
     : ProductDto(Id, Name, Price, Stock, IsActive), IRequest<Response<string>>;

