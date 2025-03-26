using Payment.Infrastructure.Interfaces;

namespace Payment.Infrastructure.Repositories
{
    public class PaymentProcessor : IPaymentProcessor
    {
        public async Task<(bool IsSuccess, string TransactionId)> ProcessPayment(Domain.Entities.Payment payment)
        {
            await Task.Delay(100); // Simulates API call delay
            return (true, Guid.NewGuid().ToString());
        }
    }
}
