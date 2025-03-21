using Payment.Infrastructure.Interfaces;
using Shared.Interfaces;

namespace Payment.Application.Validators
{
    internal class ValidatePaymentExists : IValidatePaymentExists
    {
        private readonly IValidationService _validationService;
        private readonly IPaymentRepository _paymentRepository;

        public ValidatePaymentExists(IValidationService validationService, IPaymentRepository paymentRepository)
        {
            _validationService = validationService;
            _paymentRepository = paymentRepository;
        }

        public async Task ValidatePaymentExistsAsync(int paymentId)
        {
            await _validationService.ValidateEntityExistsAsync(_paymentRepository.GetByIdAsync, paymentId, "Payment");
        }

    }
}
