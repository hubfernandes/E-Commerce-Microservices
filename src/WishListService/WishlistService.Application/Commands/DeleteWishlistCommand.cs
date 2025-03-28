using MediatR;
using Shared.Bases;

namespace WishlistService.Application.Commands
{
    public record DeleteWishlistCommand(int Id) : IRequest<Response<string>>;
}
