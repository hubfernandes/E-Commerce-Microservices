using FluentValidation;
using Payment.Application.Commands;
using Payment.Domain.Dtos;

namespace Payment.Application.Validators
{
    public class PaymentCommandValidator<T> : AbstractValidator<T> where T : PaymentDto
    {
        public PaymentCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("Order ID is required and must be a valid positive number.");

            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0).WithMessage("Payment amount cannot be negative.");

            RuleFor(x => x.PaymentMethod)
                .NotEmpty().WithMessage("Payment method is required.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Payment status is required.");
        }
    }

    public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
    {
        public CreatePaymentCommandValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0).WithMessage("Payment amount cannot be negative.");

            RuleFor(x => x.PaymentMethod)
                .NotEmpty().WithMessage("Payment method is required.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Payment status is required.");
        }
    }

    public class UpdatePaymentStatusCommandValidator : PaymentCommandValidator<UpdatePaymentStatusCommand>
    {
        public UpdatePaymentStatusCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Payment ID is required and must be a valid positive number.");

            RuleFor(x => x.TransactionId)
                .NotEmpty().When(x => x.Status == "Completed")
                .WithMessage("Transaction ID is required when payment status is 'Completed'.");
        }
    }
}

