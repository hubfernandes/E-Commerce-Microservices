using MediatR;
using Shared.Bases;

namespace InventoryService.Application.Commands
{
    public record ReserveStockCommand(int ProductId, int Quantity) : IRequest<Response<string>>;
}
