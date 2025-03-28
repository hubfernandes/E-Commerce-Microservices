namespace WishlistService.Application.Validators
{
    public interface IValidateWishlistExists
    {
        Task ValidateWishlistExistsAsync(int id);
    }
}
