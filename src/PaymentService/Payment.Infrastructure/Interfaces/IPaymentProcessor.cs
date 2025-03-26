namespace Payment.Infrastructure.Interfaces
{
    public interface IPaymentProcessor
    {
        Task<(bool IsSuccess, string TransactionId)> ProcessPayment(Domain.Entities.Payment payment);
    }

}
