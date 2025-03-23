namespace Shared.Messaging
{
    public interface IMessageBroker
    {
        Task PublishAsync<T>(string topic, T message);
        Task SubscribeAsync<T>(string topic, Func<T, Task> handler);
    }
}
