using MediatR;
using Shared.Bases;

namespace Order.Application.Commands
{
    public record CreateOrderFromCartCommand(string UserId) : IRequest<Response<string>>;
}
