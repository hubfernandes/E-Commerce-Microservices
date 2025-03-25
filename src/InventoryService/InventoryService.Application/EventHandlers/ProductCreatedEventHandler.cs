//using InventoryService.Domain.Entities;
//using InventoryService.Infrastructure.Interfaces;
//using Shared.Events;
//using Shared.Messaging;

//namespace InventoryService.Application.EventHandlers
//{
//    public class ProductCreatedEventHandler
//    {
//        private readonly IInventoryRepository _inventoryRepository;
//        private readonly IMessageBroker _messageBroker;

//        public ProductCreatedEventHandler(IInventoryRepository inventoryRepository, IMessageBroker messageBroker)
//        {
//            _inventoryRepository = inventoryRepository;
//            _messageBroker = messageBroker;

//        }

//        public async Task StartListening()
//        {
//            Console.WriteLine("🎧 Subscribing to 'product.created2'...");

//            try
//            {
//                await _messageBroker.SubscribeAsync<ProductCreatedEvent>("product.created2", Handle);
//                Console.WriteLine("✅ Successfully subscribed to 'product.created2'");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("❌ Subscription failed: {ErrorMessage}", ex.Message);
//            }
//        }


//        public async Task Handle(ProductCreatedEvent @event)
//        {
//            Console.WriteLine(" Received ProductCreatedEvent for Product ID: {ProductId}", @event.ProductId);
//            // Check if the product already exists in inventory to avoid duplication
//            var existingItem = await _inventoryRepository.GetByProductIdAsync(@event.ProductId);
//            if (existingItem != null)
//            {
//                Console.WriteLine($"Product ID {@event.ProductId} already exists in inventory.");
//                return;
//            }

//            // Create a new inventory item for the product
//            var inventoryItem = new InventoryItem(@event.ProductId, 100, 20); // Initial stock: 100, threshold: 20
//            await _inventoryRepository.AddAsync(inventoryItem);

//            Console.WriteLine($"New Inventory Item added for Product ID: {@event.ProductId}");
//        }
//    }
//}
