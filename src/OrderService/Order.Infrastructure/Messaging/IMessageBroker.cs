namespace Order.Infrastructure.Messaging
{
    public interface IMessageBroker
    {
        Task PublishAsync<T>(string topic, T message);
        void Subscribe<T>(string topic, Action<T> handler);
    }
}
