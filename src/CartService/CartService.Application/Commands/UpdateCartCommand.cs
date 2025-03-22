using CartService.Domain.Dtos;
using MediatR;
using Shared.Bases;

namespace CartService.Application.Commands
{
    public record UpdateCartCommand(int Id, string UserId, List<CartItemDto> Items)
        : CartDto(Id, UserId, Items), IRequest<Response<string>>;
}
