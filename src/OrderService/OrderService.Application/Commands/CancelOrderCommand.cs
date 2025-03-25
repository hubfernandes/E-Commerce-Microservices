using MediatR;
using Shared.Bases;

namespace OrderService.Application.Commands
{
    public record CancelOrderCommand(int OrderId) : IRequest<Response<string>>;
}
