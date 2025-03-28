using MediatR;
using Shared.Bases;
using WishlistService.Domain.Dtos;

namespace WishlistService.Application.Queries
{
    public record GetAllWishlistsQuery : IRequest<Response<List<WishlistDto>>>;
}
