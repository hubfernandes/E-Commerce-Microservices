using Microsoft.EntityFrameworkCore;
using Payment.Infrastructure.Context;
using Payment.Infrastructure.Interfaces;
using Shared.Repository;

namespace Payment.Infrastructure.Repositories
{
    public class PaymentRepository : GenericRepository<Domain.Entities.Payment>, IPaymentRepository
    {
        private readonly PaymentDbContext _context;

        public PaymentRepository(PaymentDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }
        public async Task<List<Domain.Entities.Payment>> GetByOrderIdAsync(int orderId)
        {
            return await _context.Payments
                .AsNoTracking()
                .Where(p => p.OrderId == orderId)
                .ToListAsync();
        }
    }
}
