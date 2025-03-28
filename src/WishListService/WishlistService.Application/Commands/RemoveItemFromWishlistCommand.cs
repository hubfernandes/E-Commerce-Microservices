using MediatR;
using Shared.Bases;

namespace WishlistService.Application.Commands
{
    public record RemoveItemFromWishlistCommand(string UserId, int ProductId) : IRequest<Response<string>>;
}
