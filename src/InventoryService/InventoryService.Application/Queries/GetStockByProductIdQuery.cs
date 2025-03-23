using InventoryService.Domain.Dtos;
using MediatR;
using Shared.Bases;

namespace InventoryService.Application.Queries
{
    public record GetStockByProductIdQuery(int ProductId) : IRequest<Response<StockDto>>;

}
