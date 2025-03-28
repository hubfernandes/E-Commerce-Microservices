using MediatR;
using Shared.Bases;

namespace WishlistService.Application.Commands
{
    public record DeleteWishlistByUserIdCommand(string UserId) : IRequest<Response<string>>;
}
