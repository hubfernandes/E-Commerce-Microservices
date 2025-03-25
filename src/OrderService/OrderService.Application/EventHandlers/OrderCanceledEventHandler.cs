//using InventoryService.Application.Commands;
//using MediatR;
//using OrderService.Application.Events;

//namespace OrderService.Application.EventHandlers
//{
//    public class OrderCanceledEventHandler
//    {
//        private readonly IMediator _mediator;

//        public OrderCanceledEventHandler(IMediator mediator)
//        {
//            _mediator = mediator;
//        }

//        public async Task Handle(OrderCanceledEvent @event)
//        {
//            var command = new ReleaseStockCommand(@event.ProductId, @event.Quantity);
//            await _mediator.Send(command);
//        }
//    }
//}
