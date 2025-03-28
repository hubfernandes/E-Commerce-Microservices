using FluentValidation;
using WishlistService.Application.Commands;
using WishlistService.Domain.Dtos;

namespace WishlistService.Application.Validators
{
    public class WishlistCommandValidator<T> : AbstractValidator<T> where T : WishlistDto
    {
        public WishlistCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.Items)
                .NotNull().WithMessage("Items cannot be null.");

            RuleForEach(x => x.Items)
                .ChildRules(item =>
                {
                    item.RuleFor(i => i.ProductId)
                        .GreaterThan(0).WithMessage("Product ID must be a valid positive number.");
                });
        }
    }

    //public class CreateWishlistCommandValidator : WishlistCommandValidator<CreateWishlistCommand>
    //{
    //    public CreateWishlistCommandValidator() : base() { }
    //}

    //public class UpdateWishlistCommandValidator : WishlistCommandValidator<UpdateWishlistCommand>
    //{
    //    public UpdateWishlistCommandValidator()
    //    {
    //        RuleFor(x => x.Id)
    //            .GreaterThan(0).WithMessage("Wishlist ID must be a valid positive number.");
    //    }
    //}

    public class AddItemToWishlistCommandValidator : AbstractValidator<AddItemToWishlistCommand>
    {
        public AddItemToWishlistCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
            RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("Product ID must be valid.");
        }
    }

    public class RemoveItemFromWishlistCommandValidator : AbstractValidator<RemoveItemFromWishlistCommand>
    {
        public RemoveItemFromWishlistCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
            RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("Product ID must be valid.");
        }
    }
}
