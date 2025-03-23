using MediatR;
using Shared.Bases;

namespace Order.Application.Commands
{
    public record CancelOrderCommand(int OrderId) : IRequest<Response<string>>;
}
