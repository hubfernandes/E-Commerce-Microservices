namespace ProductService.Domain.Dtos
{
    public record ProductDto(int Id, string Name, decimal Price, int Stock, bool IsActive);
}
