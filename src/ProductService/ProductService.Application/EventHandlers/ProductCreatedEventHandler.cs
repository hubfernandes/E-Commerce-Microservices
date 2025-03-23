using InventoryService.Domain.Entities;
using InventoryService.Infrastructure.Interfaces;
using ProductService.Application.Events;

namespace ProductService.Application.EventHandlers
{
    public class ProductCreatedEventHandler
    {
        private readonly IInventoryRepository _inventoryRepository;

        public ProductCreatedEventHandler(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task Handle(ProductCreatedEvent @event)
        {
            var inventoryItem = new InventoryItem(@event.ProductId, 100, 20); // Initial stock: 100, threshold: 20
            await _inventoryRepository.AddAsync(inventoryItem);
        }
    }
}
