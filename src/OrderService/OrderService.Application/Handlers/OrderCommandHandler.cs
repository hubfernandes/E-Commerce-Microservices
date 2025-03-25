using AutoMapper;
using CartService.Application.Events;
using CartService.Domain.Dtos;
using InventoryService.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using OrderService.Application.Commands;
using OrderService.Application.Events;
using OrderService.Application.Validators;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Interfaces;
using Shared.Bases;
using Shared.Messaging;
using System.Net.Http.Json;
using System.Security.Claims;
namespace OrderService.Application.Handlers
{
    public class OrderCommandHandler : IRequestHandler<CreateOrderCommand, Response<string>>,
                                       IRequestHandler<UpdateOrderCommand, Response<string>>,
                                       IRequestHandler<DeleteOrderCommand, Response<string>>,
                                       IRequestHandler<CancelOrderCommand, Response<string>>,
                                       IRequestHandler<CreateOrderFromCartCommand, Response<string>> // new one i added .. i will be check it 

    {
        private readonly IOrderRepository _orderRepository;
        private readonly IValidateOrderExists _validateOrderExists;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IMessageProducer _messageProducer;
        private readonly HttpClient _httpClientInventory;
        private readonly HttpClient _httpClientCart;
        private readonly ResponseHandler _responseHandler;
        public OrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, IValidateOrderExists validateOrderExists, ResponseHandler responseHandler,
               IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, IMessageProducer messageProducer)

        {
            _orderRepository = orderRepository;
            _httpClientInventory = httpClientFactory.CreateClient("InventoryService");
            _httpClientCart = httpClientFactory.CreateClient("CartService");
            _mapper = mapper;
            _validateOrderExists = validateOrderExists;
            _responseHandler = responseHandler;
            _httpContextAccessor = httpContextAccessor;
            _messageProducer = messageProducer;
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
                    var response = await _httpClientInventory.PostAsJsonAsync("http://localhost:5140/api/Inventory/reserve", reserveRequest);

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



        public async Task<Response<string>> Handle(CreateOrderFromCartCommand request, CancellationToken cancellationToken)
        {
            var customerId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("Customer ID not found in token.");

            var cartResponse = await _httpClientCart.GetFromJsonAsync<Response<List<CartItemDto>>>($"api/Carts/user/{request.UserId}");
            if (cartResponse == null)
                return _responseHandler.BadRequest<string>("Cart not found");

            var orderItems = cartResponse.Data!.Select(item => new OrderItem(item.ProductId, item.Quantity, item.UnitPrice)).ToList();
            var order = new Order(0, customerId, orderItems);

            await _orderRepository.AddAsync(order);

            try
            {
                foreach (var item in cartResponse.Data!)
                {
                    await _httpClientInventory.PostAsJsonAsync("api/Inventory/reserve",
                     new ReserveStockCommand(item.ProductId, item.Quantity));
                }
            }
            catch (Exception ex)
            {
                // Rollback: Publish OrderFailedEvent
                var orderFailedEvent = new OrderFailedEvent
                {
                    OrderId = order.Id,
                    Items = cartResponse.Data
                };
                await _messageProducer.PublishAsync("order.failed", orderFailedEvent);
                return _responseHandler.BadRequest<string>($"Stock deduction failed: {ex.Message}");
            }

            await _httpClientCart.DeleteAsync($"api/Carts/delete-by-UserId/{request.UserId}");
            return _responseHandler.Created<string>($"Order {order.Id} created successfully");
        }
    }
}
