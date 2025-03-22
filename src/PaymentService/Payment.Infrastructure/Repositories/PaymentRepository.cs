using Microsoft.EntityFrameworkCore;
using Payment.Infrastructure.Context;
using Payment.Infrastructure.Interfaces;
using Shared.Bases;
using Shared.Repository;
using System.Text.Json;

namespace Payment.Infrastructure.Repositories
{
    public class PaymentRepository : GenericRepository<Domain.Entities.Payment>, IPaymentRepository
    {
        private readonly PaymentDbContext _context;
        private readonly HttpClient _orderClient;
        public PaymentRepository(PaymentDbContext dbContext, IHttpClientFactory httpClientFactory) : base(dbContext)
        {
            _context = dbContext;
            _orderClient = httpClientFactory.CreateClient("OrderService")
                ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }
        public async Task<List<Domain.Entities.Payment>> GetByOrderIdAsync(int orderId)
        {
            return await _context.Payments
                .AsNoTracking()
                .Where(p => p.OrderId == orderId)
                .ToListAsync();
        }
        public override async Task<Domain.Entities.Payment> AddAsync(Domain.Entities.Payment payment)
        {
            await ValidateOrderAsync(payment.OrderId);
            var result = await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public override async Task<Domain.Entities.Payment> UpdateAsync(Domain.Entities.Payment payment)
        {
            await ValidateOrderAsync(payment.OrderId);
            return await base.UpdateAsync(payment);
        }
        private async Task ValidateOrderAsync(int orderId)
        {
            try
            {
                var response = await _orderClient.GetAsync($"api/orders/{orderId}");
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($"Order {orderId} not found in OrderService.");
                }

                var orderJson = await response.Content.ReadAsStringAsync();
                var orderResponse = JsonSerializer.Deserialize<Response<object>>(orderJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new InvalidOperationException("Failed to deserialize order response.");

                if (orderResponse.Succeeded == false)
                {
                    throw new InvalidOperationException($"Order {orderId} validation failed: {orderResponse.Message}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Failed to validate order with OrderService: {ex.Message}", ex);
            }
        }
    }
}
