using MediatR;
using ProductService.Domain.Dtos;
using Shared.Bases;

namespace ProductService.Application.Queries
{
    public record GetProductByIdQuery(int Id) : IRequest<Response<ProductDto>>;
}