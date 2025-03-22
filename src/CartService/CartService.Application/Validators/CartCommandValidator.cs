using CartService.Application.Commands;
using CartService.Domain.Dtos;
using FluentValidation;

namespace CartService.Application.Validators
{
    public class CartCommandValidator<T> : AbstractValidator<T> where T : CartDto
    {
        public CartCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Cart must contain at least one item.");

            RuleForEach(x => x.Items)
                .ChildRules(item =>
                {
                    item.RuleFor(i => i.ProductId)
                        .GreaterThan(0).WithMessage("Product ID must be a valid positive number.");
                    item.RuleFor(i => i.Quantity)
                        .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
                    item.RuleFor(i => i.UnitPrice)
                        .GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative.");
                });
        }
    }

    public class CreateCartCommandValidator : CartCommandValidator<CreateCartCommand>
    {
        public CreateCartCommandValidator() : base() { }
    }

    public class UpdateCartCommandValidator : CartCommandValidator<UpdateCartCommand>
    {
        public UpdateCartCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Cart ID must be a valid positive number.");
        }
    }
}
