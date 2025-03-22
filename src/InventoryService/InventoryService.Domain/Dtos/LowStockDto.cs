namespace InventoryService.Domain.Dtos
{
    public record LowStockDto(string ProductId, int QuantityAvailable, int QuantityReserved);
}
