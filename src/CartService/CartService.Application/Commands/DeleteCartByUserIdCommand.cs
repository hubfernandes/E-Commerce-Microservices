using MediatR;
using Shared.Bases;

namespace CartService.Application.Commands
{
    public record DeleteCartByUserIdCommand(string UserId) : IRequest<Response<string>>;

}
