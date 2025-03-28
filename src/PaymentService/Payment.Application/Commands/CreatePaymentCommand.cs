using MediatR;
using Payment.Domain.Entities;
using Shared.Bases;

namespace Payment.Application.Commands
{
    public record CreatePaymentCommand(int OrderId, decimal Amount, PaymentStatus Status, PaymentMethodType PaymentMethod) : IRequest<Response<string>>;

}
