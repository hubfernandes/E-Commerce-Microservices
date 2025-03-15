namespace ProductService.Domain.Entities
{
    public class Product
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public int Stock { get; private set; }
        public bool IsActive { get; private set; }

        public Product(int id, string name, decimal price, int stock)
        {
            Id = id;
            Name = name;
            Price = price;
            Stock = stock;
            IsActive = true;
        }

        public void UpdateStock(int quantityChange)
        {
            if (!IsActive)
                throw new InvalidOperationException("Cannot update stock for an inactive product.");
            if (Stock + quantityChange < 0)
                throw new InvalidOperationException("Stock cannot go below zero.");

            Stock += quantityChange;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}