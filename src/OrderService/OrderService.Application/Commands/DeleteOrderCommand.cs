using MediatR;
using Shared.Bases;

namespace OrderService.Application.Commands
{
    public record DeleteOrderCommand(int Id) : IRequest<Response<string>>;
}
