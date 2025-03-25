using InventoryService.Domain.Entities;
using InventoryService.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Events;
using System.Text;
using System.Text.Json;

namespace InventoryService.Application.BackgroundServices
{
    public class ProductCreatedEventConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ProductCreatedEventConsumer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("🎧 Subscribing to 'product.created3'...");
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = "localhost"
                };

                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                channel.QueueDeclare(queue: "product.created3", exclusive: false);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (sender, args) =>
                {
                    using var scope = _scopeFactory.CreateScope(); //  Create scope inside the event handler
                    var inventoryRepository = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

                    var body = args.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"📩 Message received: {message}");

                    var productEvent = JsonSerializer.Deserialize<ProductCreatedEvent>(message);
                    if (productEvent == null)
                    {
                        Console.WriteLine("❌ Failed to deserialize message.");
                        return;
                    }

                    var inventoryItem = new InventoryItem(productEvent.ProductId, 100, 20); // Initial stock: 100, threshold: 20
                    await inventoryRepository.AddAsync(inventoryItem);

                    Console.WriteLine($"✅ New Inventory Item added for Product ID: {productEvent.ProductId}");
                };

                channel.BasicConsume(queue: "product.created3", autoAck: true, consumer: consumer);

                await Task.Delay(Timeout.Infinite, stoppingToken); // Keep background service running
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Subscription failed: {ex.Message}");
            }
        }
    }
}
