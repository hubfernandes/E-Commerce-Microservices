using MediatR;
using Shared.Bases;

namespace InventoryService.Application.Commands
{
    public record AddStockCommand(int ProductId, int Quantity, int LowStockThreshold) : IRequest<Response<string>>;
}
