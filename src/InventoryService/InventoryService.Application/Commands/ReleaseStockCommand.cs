using MediatR;
using Shared.Bases;

namespace InventoryService.Application.Commands
{
    public record ReleaseStockCommand(int ProductId, int Quantity) : IRequest<Response<string>>;
}
