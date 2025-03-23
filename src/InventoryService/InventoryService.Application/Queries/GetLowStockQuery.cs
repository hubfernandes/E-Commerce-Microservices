using InventoryService.Domain.Dtos;
using MediatR;
using Shared.Bases;

namespace InventoryService.Application.Queries
{
    public record GetLowStockQuery(int Threshold) : IRequest<Response<List<LowStockDto>>>;
}
