using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderService.Application.Events;
using Payment.Application.Events;
using Payment.Domain.Entities;
using Payment.Infrastructure.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Messaging;
using System.Text;
using System.Text.Json;

namespace Payment.Application.BackgroundServices
{
    public class OrderCreatedEventConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public OrderCreatedEventConsumer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("🎧 Subscribing to 'order.created'...");

            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };

                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                channel.QueueDeclare(queue: "order.created", exclusive: false);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (sender, args) =>
                {
                    using var scope = _scopeFactory.CreateScope();
                    var paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();
                    var paymentProcessor = scope.ServiceProvider.GetRequiredService<IPaymentProcessor>();
                    var messageProducer = scope.ServiceProvider.GetRequiredService<IMessageProducer>();

                    var body = args.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"📩 Received order event: {message}");

                    var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(message);
                    if (orderEvent == null)
                    {
                        Console.WriteLine("❌ Failed to deserialize OrderCreatedEvent.");
                        return;
                    }

                    var payment = new Payment.Domain.Entities.Payment
                    {
                        OrderId = orderEvent.OrderId,
                        Amount = orderEvent.TotalAmount
                    };

                    var paymentResult = await paymentProcessor.ProcessPayment(payment);

                    if (paymentResult.IsSuccess)
                    {
                        payment.UpdateStatus(PaymentStatus.Completed, paymentResult.TransactionId);
                    }
                    else
                    {
                        payment.UpdateStatus(PaymentStatus.Failed, null!);
                    }

                    await paymentRepository.AddAsync(payment);

                    // Publish PaymentProcessedEvent
                    var paymentProcessedEvent = new PaymentProcessedEvent(orderEvent.OrderId, payment.Status);
                    await messageProducer.PublishAsync("payment.processed", paymentProcessedEvent);

                    Console.WriteLine($"✅ Payment processed for Order ID: {orderEvent.OrderId}");
                };

                channel.BasicConsume(queue: "order.created", autoAck: true, consumer: consumer);

                await Task.Delay(Timeout.Infinite, stoppingToken); // Keep the consumer running
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Subscription failed: {ex.Message}");
            }
        }
    }
}
