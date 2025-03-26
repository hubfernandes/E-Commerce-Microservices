using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities
{
    public class Order
    {
        public int Id { get; private set; }
        public string? CustomerId { get; set; }
        public DateTime OrderDate { get; private set; }
        public decimal TotalAmount { get; private set; }
        public OrderStatus Status { get; set; }
        public List<OrderItem> Items { get; private set; } = new List<OrderItem>();
        private Order()
        {
        }
        public Order(int id, string customerId, List<OrderItem> items)
        {
            if (items == null || !items.Any())
                throw new ArgumentException("Order must contain at least one item.", nameof(items));

            Id = id;
            CustomerId = customerId;
            OrderDate = DateTime.UtcNow;
            Items = items;
            Status = OrderStatus.Pending;
            CalculateTotalAmount();
        }

        public void CalculateTotalAmount()
        {
            TotalAmount = Items.Sum(item => item.UnitPrice * item.Quantity);
        }

        public void UpdateStatus(OrderStatus newStatus)
        {
            if (Status == OrderStatus.Completed && newStatus != OrderStatus.Completed)
                throw new InvalidOperationException("Completed orders cannot change status.");
            Status = newStatus;
            CalculateTotalAmount();
        }
    }
}
