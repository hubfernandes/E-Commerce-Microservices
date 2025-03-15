namespace ProductService.Application.Validators
{
    public interface IValidateProductExists
    {
        Task ValidateProductExistsAsync(int productId);
    }
}
