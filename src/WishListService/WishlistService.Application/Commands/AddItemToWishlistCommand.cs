using MediatR;
using Shared.Bases;

namespace WishlistService.Application.Commands
{
    public record AddItemToWishlistCommand(string UserId, int ProductId) : IRequest<Response<string>>;
}
