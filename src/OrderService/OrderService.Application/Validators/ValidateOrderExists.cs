using OrderService.Infrastructure.Interfaces;
using Shared.Interfaces;
namespace OrderService.Application.Validators
{
    internal class ValidateOrderExists : IValidateOrderExists
    {

        private readonly IValidationService _validationService;
        private readonly IOrderRepository _orderRepository;

        public ValidateOrderExists(IValidationService validationService, IOrderRepository orderRepository)
        {
            _validationService = validationService;
            _orderRepository = orderRepository;
        }
        public async Task ValidateOrderExistsAsync(int orderId)
        {
            await _validationService.ValidateEntityExistsAsync(_orderRepository.GetByIdAsync, orderId, "Order");
        }
    }
}
