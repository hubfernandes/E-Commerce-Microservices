namespace InventoryService.Application.Commands
{
    public record ReserveStockCommand(string ProductId, int Quantity);
}
