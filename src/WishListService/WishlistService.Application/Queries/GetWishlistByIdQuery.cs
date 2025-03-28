using MediatR;
using Shared.Bases;
using WishlistService.Domain.Dtos;

namespace WishlistService.Application.Queries
{
    public record GetWishlistByIdQuery(int Id) : IRequest<Response<WishlistDto>>;
}
