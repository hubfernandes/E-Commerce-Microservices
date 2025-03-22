namespace InventoryService.Domain.Dtos
{
    public record StockDto(string ProductId, int QuantityAvailable, int QuantityReserved, string Status);
}
