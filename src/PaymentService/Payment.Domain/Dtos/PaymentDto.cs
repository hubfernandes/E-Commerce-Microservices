namespace Payment.Domain.Dtos
{
    public record PaymentDto(int Id, int OrderId, decimal Amount, string? PaymentMethod, string Status, string? TransactionId);
}
