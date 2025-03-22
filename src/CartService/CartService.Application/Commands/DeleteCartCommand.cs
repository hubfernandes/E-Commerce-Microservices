using MediatR;
using Shared.Bases;

namespace CartService.Application.Commands
{
    public record DeleteCartCommand(int Id) : IRequest<Response<string>>;
}
