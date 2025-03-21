namespace Payment.Application.Validators
{
    public interface IValidatePaymentExists
    {
        Task ValidatePaymentExistsAsync(int paymentId);
    }
}
