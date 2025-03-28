using MediatR;
using Shared.Bases;
using WishlistService.Domain.Dtos;

namespace WishlistService.Application.Queries
{
    public record GetWishlistByUserIdQuery(string UserId) : IRequest<Response<WishlistDto>>;
}
