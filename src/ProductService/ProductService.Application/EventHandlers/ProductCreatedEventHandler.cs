using InventoryService.Domain.Entities;
using InventoryService.Infrastructure.Interfaces;
using ProductService.Application.Events;
using Shared.Messaging;

namespace ProductService.Application.EventHandlers
{
    public class ProductCreatedEventHandler
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMessageBroker _messageBroker;

        public ProductCreatedEventHandler(IInventoryRepository inventoryRepository, IMessageBroker messageBroker)
        {
            _inventoryRepository = inventoryRepository;
            _messageBroker = messageBroker;
        }

        public async Task StartListening()
        {
            await _messageBroker.SubscribeAsync<ProductCreatedEvent>("product.created", Handle);
        }

        public async Task Handle(ProductCreatedEvent @event)
        {
            var inventoryItem = new InventoryItem(@event.ProductId, 100, 20); // Initial stock: 100, threshold: 20
            await _inventoryRepository.AddAsync(inventoryItem);
        }
    }
}
