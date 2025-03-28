using MediatR;
using Shared.Bases;
using WishlistService.Domain.Dtos;

namespace WishlistService.Application.Commands
{
    public record CreateWishlistCommand(string UserId, List<WishlistItemDto> Items) : IRequest<Response<string>>;
}
