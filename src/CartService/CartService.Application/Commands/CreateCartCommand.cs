using CartService.Domain.Dtos;
using MediatR;
using Shared.Bases;

namespace CartService.Application.Commands
{
    public record CreateCartCommand(string UserId, List<CartItemDto> Items)
          : CartDto(0, UserId, Items), IRequest<Response<string>>;
}
