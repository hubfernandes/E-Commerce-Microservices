using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
namespace Shared.Messaging;

public class RabbitMQMessageBroker : IMessageBroker
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    public RabbitMQMessageBroker(string hostname = "localhost")
    {
        var factory = new ConnectionFactory { HostName = hostname };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public async Task PublishAsync<T>(string topic, T message)
    {
        _channel.QueueDeclare(queue: topic, durable: true, exclusive: false, autoDelete: false, arguments: null);
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        _channel.BasicPublish(exchange: "", routingKey: topic, basicProperties: properties, body: body);
        await Task.CompletedTask; // No actual async operation, just return a completed task
    }

    public async Task SubscribeAsync<T>(string topic, Func<T, Task> handler)
    {
        _channel.QueueDeclare(queue: topic, durable: true, exclusive: false, autoDelete: false, arguments: null);
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(body));
            if (message != null)
            {
                await handler(message);
            }
            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(queue: topic, autoAck: false, consumer: consumer);
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}
