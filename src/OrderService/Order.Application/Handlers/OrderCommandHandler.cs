using AutoMapper;
using MediatR;
using Order.Application.Commands;
using Order.Application.Validators;
using Order.Infrastructure.Interfaces;
using Shared.Bases;

namespace Order.Application.Handlers
{
    public class OrderCommandHandler :
         IRequestHandler<CreateOrderCommand, Response<string>>,
         IRequestHandler<UpdateOrderCommand, Response<string>>,
         IRequestHandler<DeleteOrderCommand, Response<string>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IValidateOrderExists _validateOrderExists;
        private readonly ResponseHandler _responseHandler;

        public OrderCommandHandler(
            IOrderRepository orderRepository,
            IMapper mapper,
            IValidateOrderExists validateOrderExists,
            ResponseHandler responseHandler)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _validateOrderExists = validateOrderExists;
            _responseHandler = responseHandler;
        }

        public async Task<Response<string>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var order = _mapper.Map<Order.Domain.Entities.Order>(request);
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
                var order = _mapper.Map<Order.Domain.Entities.Order>(request);
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
    }
}
