using ProductService.Infrastructure.Interfaces;
using Shared.Interfaces;

namespace ProductService.Application.Validators
{
    internal class ValidateProductExists : IValidateProductExists
    {
        private readonly IValidationService _validationService;
        private readonly IProductRepository _productRepository;

        public ValidateProductExists(IValidationService validationService, IProductRepository productRepository)
        {
            _validationService = validationService;
            _productRepository = productRepository;
        }
        public async Task ValidateProductExistsAsync(int productId)
        {
            await _validationService.ValidateEntityExistsAsync(_productRepository.GetByIdAsync, productId, "Product");
        }
    }
}
