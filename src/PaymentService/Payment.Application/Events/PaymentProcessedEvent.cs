using Payment.Domain.Entities;

namespace Payment.Application.Events
{
    public record PaymentProcessedEvent(int OrderId, PaymentStatus Status);
}
