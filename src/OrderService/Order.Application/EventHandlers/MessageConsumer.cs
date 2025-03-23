using Microsoft.Extensions.Hosting;
using Order.Application.Events;
using Order.Infrastructure.Messaging;

namespace Order.Application.EventHandlers
{
    public class MessageConsumer : IHostedService
    {
        private readonly IMessageBroker _messageBroker;
        private readonly OrderCanceledEventHandler _handler;

        public MessageConsumer(IMessageBroker messageBroker, OrderCanceledEventHandler handler)
        {
            _messageBroker = messageBroker;
            _handler = handler;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _messageBroker.Subscribe<OrderCanceledEvent>("order.canceled", async (evt) => await _handler.Handle(evt));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
