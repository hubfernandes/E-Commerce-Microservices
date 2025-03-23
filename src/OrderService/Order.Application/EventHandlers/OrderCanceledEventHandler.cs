using InventoryService.Application.Commands;
using MediatR;
using Order.Application.Events;

namespace Order.Application.EventHandlers
{
    public class OrderCanceledEventHandler
    {
        private readonly IMediator _mediator;

        public OrderCanceledEventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(OrderCanceledEvent @event)
        {
            var command = new ReleaseStockCommand(@event.ProductId, @event.Quantity);
            await _mediator.Send(command);
        }
    }
}
