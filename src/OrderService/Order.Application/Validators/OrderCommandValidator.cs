using FluentValidation;
using Order.Application.Commands;
using Order.Domain.Dtos;

namespace Order.Application.Validators
{
    public class OrderCommandValidator<T> : AbstractValidator<T> where T : OrderDto
    {
        public OrderCommandValidator()
        {
            RuleFor(x => x.CustomerId)
                 .NotEmpty().WithMessage("Customer ID is required .");

            RuleFor(x => x.OrderDate)
                .NotEmpty().WithMessage("Order date is required.");

            RuleFor(x => x.TotalAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Total amount cannot be negative.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Order must contain at least one item.");
        }
    }

    public class CreateOrderCommandValidator : OrderCommandValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator() : base() { }
    }

    public class UpdateOrderCommandValidator : OrderCommandValidator<UpdateOrderCommand>
    {
        public UpdateOrderCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Order ID is required and must be a valid positive number.");
        }
    }
}
