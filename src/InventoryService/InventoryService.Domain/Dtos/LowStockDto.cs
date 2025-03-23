namespace InventoryService.Domain.Dtos
{
    public record LowStockDto(int ProductId, int QuantityAvailable, int QuantityReserved);
}
