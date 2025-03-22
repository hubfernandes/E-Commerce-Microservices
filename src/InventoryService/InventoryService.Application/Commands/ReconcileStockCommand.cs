namespace InventoryService.Application.Commands
{
    public record ReconcileStockCommand(string ProductId, int Quantity, string Reason);
}
