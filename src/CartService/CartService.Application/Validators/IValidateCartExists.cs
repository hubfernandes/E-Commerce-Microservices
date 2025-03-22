namespace CartService.Application.Validators
{
    public interface IValidateCartExists
    {
        Task ValidateCartExistsAsync(int cartId);
    }
}
