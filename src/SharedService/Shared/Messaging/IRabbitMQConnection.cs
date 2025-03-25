using RabbitMQ.Client;

namespace Shared.Messaging
{
    public interface IRabbitMQConnection
    {
        public IConnection Connection { get; }
    }
}
