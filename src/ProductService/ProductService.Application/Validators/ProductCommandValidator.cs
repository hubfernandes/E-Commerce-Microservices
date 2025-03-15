using FluentValidation;
using ProductService.Application.Commands;
using ProductService.Domain.Dtos;

namespace ProductService.Application.Validators
{
    public class ProductCommandValidator<T> : AbstractValidator<T> where T : ProductDto
    {
        public ProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");
        }
    }

    public class CreateProductCommandValidator : ProductCommandValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator() : base() { }
    }

    public class UpdateProductCommandValidator : ProductCommandValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Product ID is required and must be a valid positive number.");
        }
    }
}