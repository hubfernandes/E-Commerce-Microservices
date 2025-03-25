namespace OrderService.Application.Validators
{
    public interface IValidateOrderExists
    {
        Task ValidateOrderExistsAsync(int orderId);
    }
}
