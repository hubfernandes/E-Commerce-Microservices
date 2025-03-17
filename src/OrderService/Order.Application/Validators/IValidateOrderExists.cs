namespace Order.Application.Validators
{
    public interface IValidateOrderExists
    {
        Task ValidateOrderExistsAsync(int orderId);
    }
}
