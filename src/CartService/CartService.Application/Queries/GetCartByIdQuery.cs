using CartService.Domain.Dtos;
using MediatR;
using Shared.Bases;

namespace CartService.Application.Queries
{
    public record GetCartByIdQuery(int Id) : IRequest<Response<CartDto>>;
}
