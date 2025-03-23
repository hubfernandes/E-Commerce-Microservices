namespace ProductService.Application.Events
{
    public record ProductCreatedEvent(int ProductId, string Name);
}
