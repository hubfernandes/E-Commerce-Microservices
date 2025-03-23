//using Microsoft.EntityFrameworkCore.Metadata;
//using Order.Infrastructure.Messaging;
//using RabbitMQ.Client;
//using System.Text;
//using System.Text.Json;

//public class RabbitMQMessageBroker : IMessageBroker
//{
//    private readonly IConnection _connection;
//    private readonly IModel _channel;

//    public RabbitMQMessageBroker(string hostname = "localhost")
//    {
//        var factory = new ConnectionFactory() { HostName = hostname };
//        _connection = (IConnection)factory.CreateConnectionAsync()!;
//        _channel = _connection.CreateModel();
//    }

//    public async Task PublishAsync<T>(string topic, T message)
//    {
//        _channel.QueueDeclare(queue: topic, durable: false, exclusive: false, autoDelete: false, arguments: null);
//        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
//        await Task.Run(() => _channel.BasicPublish(exchange: "", routingKey: topic, basicProperties: null, body: body));
//    }

//    public void Subscribe<T>(string topic, Action<T> handler)
//    {
//        _channel.QueueDeclare(queue: topic, durable: false, exclusive: false, autoDelete: false, arguments: null);
//        var consumer = new EventingBasicConsumer(_channel);
//        consumer.Received += (model, ea) =>
//        {
//            var body = ea.Body.ToArray();
//            var message = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(body));
//            handler?.Invoke(message);
//        };
//        _channel.BasicConsume(queue: topic, autoAck: true, consumer: consumer);
//    }

//    public void Dispose()
//    {
//        _channel?.Close();
//        _connection?.Close();
//    }
//}
