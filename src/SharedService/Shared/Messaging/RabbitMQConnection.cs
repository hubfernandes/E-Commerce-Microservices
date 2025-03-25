using RabbitMQ.Client;

namespace Shared.Messaging
{
    public class RabbitMQConnection : IRabbitMQConnection, IDisposable
    {
        public IConnection _connection;
        public IConnection Connection => _connection;

        public RabbitMQConnection(string hostname = "localhost")
        {
            var factory = new ConnectionFactory { HostName = hostname };
            _connection = factory.CreateConnection();
        }
        public void Dispose()
        {
            _connection?.Close();
        }
    }


}
