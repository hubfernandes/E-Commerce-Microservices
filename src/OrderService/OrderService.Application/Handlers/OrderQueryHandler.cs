using AutoMapper;
using MediatR;
using OrderService.Application.Queries;
using OrderService.Domain.Dtos;
using OrderService.Infrastructure.Interfaces;
using Shared.Bases;

namespace OrderService.Application.Handlers
{
    public class OrderQueryHandler :
        IRequestHandler<GetOrderByIdQuery, Response<OrderDto>>,
        IRequestHandler<GetAllOrdersQuery, Response<List<OrderDto>>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ResponseHandler _responseHandler;

        public OrderQueryHandler(
            IOrderRepository orderRepository,
            IMapper mapper,
            ResponseHandler responseHandler)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _responseHandler = responseHandler;
        }

        public async Task<Response<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.Id);
            if (order == null)
                return _responseHandler.NotFound<OrderDto>("Order not found");

            var mappedOrder = _mapper.Map<OrderDto>(order);
            return _responseHandler.Success(mappedOrder);
        }

        public async Task<Response<List<OrderDto>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetAllAsync();
            var mappedOrders = _mapper.Map<List<OrderDto>>(orders);
            return _responseHandler.Success(mappedOrders);
        }
    }
}
