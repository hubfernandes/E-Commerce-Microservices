using RabbitMQ.Client;

using System.Text;
using System.Text.Json;

namespace Shared.Messaging
{
    internal class RabbitMQProducer : IMessageProducer
    {
        private readonly IRabbitMQConnection _connection;

        public RabbitMQProducer(IRabbitMQConnection connection)
        {
            _connection = connection;
        }

        //public void SendAsync<T>(T message) where T : class
        //{
        //    using var channel = _connection.Connection.CreateModel();
        //    channel.QueueDeclare("product.created3", exclusive: false);
        //    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        //    channel.BasicPublish(exchange: "", routingKey: "product.created3", basicProperties: null, body: body);
        //}

        public async Task PublishAsync<T>(string queueName, T message) where T : class
        {
            using var channel = _connection.Connection.CreateModel();
            channel.QueueDeclare(queue: queueName, exclusive: false);
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
            await Task.CompletedTask;
        }

    }
}
