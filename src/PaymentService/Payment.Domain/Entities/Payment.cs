namespace Payment.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethodType PaymentMethod { get; set; } = PaymentMethodType.CreditCard;
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string? TransactionId { get; private set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;



        public void UpdateStatus(PaymentStatus status, string transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
            {
                throw new ArgumentException("TransactionId cannot be null or empty", nameof(transactionId));
            }

            Status = status;
            TransactionId = transactionId;
        }
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed
    }

    public enum PaymentMethodType
    {
        CreditCard,
        PayPal
    }
}
