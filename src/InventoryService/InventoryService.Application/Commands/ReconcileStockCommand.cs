using MediatR;
using Shared.Bases;

namespace InventoryService.Application.Commands
{
    public record ReconcileStockCommand(int ProductId, int Quantity, string Reason) : IRequest<Response<string>>;
}
