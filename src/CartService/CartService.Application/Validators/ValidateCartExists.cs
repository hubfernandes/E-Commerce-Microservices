using CartService.Infrastructure.Interfaces;
using Shared.Interfaces;

namespace CartService.Application.Validators
{
    internal class ValidateCartExists : IValidateCartExists
    {
        private readonly IValidationService _validationService;
        private readonly ICartRepository _cartRepository;

        public ValidateCartExists(IValidationService validationService, ICartRepository cartRepository)
        {
            _validationService = validationService;
            _cartRepository = cartRepository;
        }

        public async Task ValidateCartExistsAsync(int cartId)
        {
            await _validationService.ValidateEntityExistsAsync(_cartRepository.GetByIdAsync, cartId, "Cart");
        }
    }
}
