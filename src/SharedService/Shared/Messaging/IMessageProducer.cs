namespace Shared.Messaging
{
    public interface IMessageProducer
    {
        Task PublishAsync<T>(string queueName, T message) where T : class;
    }
}
