using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Payment.Application.Events;
using Payment.Infrastructure.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Payment.Application.BackgroundServices
{
    public class PaymentProcessedEventConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public PaymentProcessedEventConsumer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("🎧 Subscribing to 'payment.processed'...");

            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };

                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                channel.QueueDeclare(queue: "payment.processed", exclusive: false);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (sender, args) =>
                {
                    using var scope = _scopeFactory.CreateScope();
                    var paymetRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();

                    var body = args.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"📩 Received payment event: {message}");

                    var paymentEvent = JsonSerializer.Deserialize<PaymentProcessedEvent>(message);
                    if (paymentEvent == null)
                    {
                        Console.WriteLine("❌ Failed to deserialize PaymentProcessedEvent.");
                        return;
                    }

                    var payment = await paymetRepository.GetByIdAsync(paymentEvent.OrderId);
                    if (payment == null) return;

                    payment.UpdateStatus(paymentEvent.Status, payment.TransactionId!);
                    await paymetRepository.UpdateAsync(payment);

                    Console.WriteLine($"✅ Order {paymentEvent.OrderId} updated with payment status: {paymentEvent.Status}");
                };

                channel.BasicConsume(queue: "payment.processed", autoAck: true, consumer: consumer);

                await Task.Delay(Timeout.Infinite, stoppingToken); // Keep the consumer running
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Subscription failed: {ex.Message}");
            }
        }
    }
}
