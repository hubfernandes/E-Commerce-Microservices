using MediatR;
using Shared.Bases;

namespace OrderService.Application.Commands
{
    public record CreateOrderFromCartCommand(string UserId) : IRequest<Response<string>>;
}
