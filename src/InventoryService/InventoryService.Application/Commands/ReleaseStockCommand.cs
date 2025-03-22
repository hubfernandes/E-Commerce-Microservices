namespace InventoryService.Application.Commands
{
    public record ReleaseStockCommand(string ProductId, int Quantity);
}
