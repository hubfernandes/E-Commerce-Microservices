using MediatR;
using Shared.Bases;

namespace InventoryService.Application.Commands
{
    public record UpdateStockCommand(int ProductId, int Quantity) : IRequest<Response<string>>;
}
