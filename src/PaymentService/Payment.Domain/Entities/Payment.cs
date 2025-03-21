namespace Payment.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; } // e.g., "CreditCard", "PayPal"
        public string? Status { get; set; } // e.g., "Pending", "Completed", "Failed"
        public string? TransactionId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public void UpdateStatus(string status, string transactionId)
        {
            Status = status;
            TransactionId = transactionId;
        }
    }
}
