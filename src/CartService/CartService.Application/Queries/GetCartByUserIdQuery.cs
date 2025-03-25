using CartService.Domain.Dtos;
using MediatR;
using Shared.Bases;

namespace CartService.Application.Queries
{
    public record GetCartsByUserIdQuery(string UserId) : IRequest<Response<List<CartItemDto>>>;
}
