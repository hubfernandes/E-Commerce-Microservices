using AutoMapper;
using InventoryService.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Order.Application.Commands;
using Order.Application.Events;
using Order.Application.Validators;
using Order.Infrastructure.Interfaces;
using Shared.Bases;
using System.Net.Http.Json;
using System.Security.Claims;
namespace Order.Application.Handlers
{
    public class OrderCommandHandler : IRequestHandler<CreateOrderCommand, Response<string>>,
                                       IRequestHandler<UpdateOrderCommand, Response<string>>,
                                       IRequestHandler<DeleteOrderCommand, Response<string>>,
                                       IRequestHandler<CancelOrderCommand, Response<string>>
    //  IRequestHandler<CreateOrderFromCartCommand, Response<string>> // new one i added .. i will be check it 

    {
        private readonly IOrderRepository _orderRepository;
        private readonly IValidateOrderExists _validateOrderExists;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //  private readonly IMessageBroker _messageBroker;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly ResponseHandler _responseHandler;

        public OrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, IValidateOrderExists validateOrderExists, ResponseHandler responseHandler,
               IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)

        {
            _orderRepository = orderRepository;
            _httpClient = httpClientFactory.CreateClient("InventoryService");
            _mapper = mapper;
            _validateOrderExists = validateOrderExists;
            _responseHandler = responseHandler;
            _httpContextAccessor = httpContextAccessor;
            // _messageBroker = messageBroker;
        }

        public async Task<Response<string>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var customerId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? throw new UnauthorizedAccessException("Customer ID not found in token.");

                // Reserve stock for each item in the order
                foreach (var item in request.Items)
                {
                    var reserveRequest = new ReserveStockCommand(item.ProductId, item.Quantity);
                    var response = await _httpClient.PostAsJsonAsync("http://localhost:5140/api/Inventory/reserve", reserveRequest);

                    if (!response.IsSuccessStatusCode)
                    {
                        return _responseHandler.BadRequest<string>($"Failed to reserve stock for ProductId: {item.ProductId}");
                    }
                }


                var order = _mapper.Map<Domain.Entities.Order>(request);
                order.CustomerId = customerId;

                var addedOrder = await _orderRepository.AddAsync(order);
                return _responseHandler.Created<string>($"Order {addedOrder.Id} Created Successfully");
            }
            catch (Exception ex)
            {
                return _responseHandler.BadRequest<string>(ex.Message);
            }
        }

        public async Task<Response<string>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _validateOrderExists.ValidateOrderExistsAsync(request.Id);
                var order = _mapper.Map<Domain.Entities.Order>(request);
                await _orderRepository.UpdateAsync(order);
                return _responseHandler.Success<string>("Order Updated Successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return _responseHandler.NotFound<string>(ex.Message);
            }
        }

        public async Task<Response<string>> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _validateOrderExists.ValidateOrderExistsAsync(request.Id);
                await _orderRepository.DeleteByIdAsync(request.Id);
                return _responseHandler.Success<string>("Order Deleted Successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return _responseHandler.NotFound<string>(ex.Message);
            }
        }

        public async Task<Response<string>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            if (order == null) return _responseHandler.NotFound<string>("Order not found");

            await _orderRepository.DeleteAsync(order);

            foreach (var x in order.Items)  // publish event to release stock
            {
                var releaseEvent = new OrderCanceledEvent(x.ProductId, x.Quantity);
                //   await _messageBroker.PublishAsync("order.canceled", releaseEvent);
            }

            return _responseHandler.Success<string>("Order canceled successfully");
        }






        //public async Task<Response<string>> Handle(CreateOrderFromCartCommand request, CancellationToken cancellationToken)
        //{
        //    var cart = await _cartRepository.GetByUserIdAsync(request.UserId);
        //    if (cart == null || !cart.Items.Any()) return _responseHandler.BadRequest<string>("Cart is empty");

        //    // Convert cart to order
        //    var order = new Order(cart.Items.Select(i => new OrderItem(i.ProductId, i.Quantity)).ToList());
        //    await _orderRepository.AddAsync(order);

        //    // Deduct reserved stock permanently
        //    foreach (var item in cart.Items)
        //    {
        //        var updateRequest = new UpdateStockCommand(item.ProductId, -item.Quantity); // Negative to deduct
        //        var response = await httpClientFactory.PutAsJsonAsync("http://inventory-service/api/inventory/update", updateRequest);
        //        if (!response.IsSuccessStatusCode) return _responseHandler.BadRequest<string>("Failed to update stock");
        //    }

        //    // Clear cart
        //    await _cartRepository.DeleteAsync(cart);
        //    return _responseHandler.Created<string>("Order created successfully");
        //}
    }
}
