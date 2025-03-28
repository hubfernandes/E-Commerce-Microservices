using Shared.Interfaces;
using WishlistService.Infrastructure.Interfaces;

namespace WishlistService.Application.Validators
{
    public class ValidateWishlistExists : IValidateWishlistExists
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IValidationService _validationService;

        public ValidateWishlistExists(IValidationService validationService, IWishlistRepository wishlistRepository)
        {
            _validationService = validationService;
            _wishlistRepository = wishlistRepository;
        }

        public async Task ValidateWishlistExistsAsync(int cartId)
        {
            await _validationService.ValidateEntityExistsAsync(_wishlistRepository.GetByIdAsync, cartId, "WishList");
        }

    }
}
