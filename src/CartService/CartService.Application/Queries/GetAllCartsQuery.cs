using CartService.Domain.Dtos;
using MediatR;
using Shared.Bases;

namespace CartService.Application.Queries
{
    public record GetAllCartsQuery : IRequest<Response<List<CartDto>>>;
}
