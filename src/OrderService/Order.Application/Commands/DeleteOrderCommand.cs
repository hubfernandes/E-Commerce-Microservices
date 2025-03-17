using MediatR;
using Shared.Bases;

namespace Order.Application.Commands
{
    public record DeleteOrderCommand(int Id) : IRequest<Response<string>>;
}
