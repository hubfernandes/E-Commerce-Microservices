using AutoMapper;
using InventoryService.Domain.Dtos;
using InventoryService.Domain.Entities;

namespace InventoryService.Application.Mappings
{
    public class InventoryProfile : Profile
    {
        public InventoryProfile()
        {
            CreateMap<InventoryItem, StockDto>();
            CreateMap<InventoryItem, LowStockDto>();
        }
    }
}
