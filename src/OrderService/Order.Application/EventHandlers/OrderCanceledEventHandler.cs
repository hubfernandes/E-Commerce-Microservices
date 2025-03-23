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
            foreach (var item in @event.Items)
            {
                var command = new ReleaseStockCommand(item.ProductId, item.Quantity);
                await _mediator.Send(command);
            }
        }
    }
}
