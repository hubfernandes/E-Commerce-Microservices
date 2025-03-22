namespace InventoryService.Application.Commands
{
    public record UpdateStockCommand(string ProductId, int Quantity);
}
