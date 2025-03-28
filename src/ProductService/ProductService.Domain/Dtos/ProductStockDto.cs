namespace ProductService.Domain.Dtos
{
    public record ProductStockDto(int ProductId, string? Name, int QuantityAvailable);
}
