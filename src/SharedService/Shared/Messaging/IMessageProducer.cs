namespace Shared.Messaging
{
    public interface IMessageProducer
    {
        void SendAsync<T>(T message) where T : class;
    }
}
