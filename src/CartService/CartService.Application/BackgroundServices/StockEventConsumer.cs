using CartService.Application.Events;
using InventoryService.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace CartService.Application.BackgroundServices
{
    public class StockEventConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public StockEventConsumer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "order.failed", durable: true, exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (sender, args) =>
            {
                using var scope = _scopeFactory.CreateScope();
                var inventoryRepository = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    if (args.RoutingKey == "order.failed")
                    {
                        var orderFailedEvent = JsonSerializer.Deserialize<OrderFailedEvent>(message);
                        if (orderFailedEvent != null)
                        {
                            foreach (var item in orderFailedEvent.Items)
                            {
                                var inventoryItem = await inventoryRepository.GetByProductIdAsync(item.ProductId);
                                if (inventoryItem != null)
                                {
                                    inventoryItem.ReleaseStock(item.Quantity);
                                    await inventoryRepository.UpdateAsync(inventoryItem);
                                }
                                else
                                {
                                    Console.WriteLine($"⚠️ Inventory item not found for ProductId: {item.ProductId}");
                                }
                            }
                            Console.WriteLine($"✅ Stock released for failed order: {orderFailedEvent.OrderId}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error processing order.failed event: {ex.Message}");
                }
            };

            channel.BasicConsume(queue: "order.failed", autoAck: true, consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}