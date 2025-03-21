using Shared.Interfaces;

namespace Payment.Infrastructure.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Domain.Entities.Payment>
    {
        Task<List<Domain.Entities.Payment>> GetByOrderIdAsync(int orderId);
    }
}
