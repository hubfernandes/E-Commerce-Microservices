using MediatR;
using Payment.Domain.Dtos;
using Shared.Bases;

namespace Payment.Application.Commands
{
    public record CreatePaymentCommand(int OrderId, decimal Amount, string PaymentMethod)
         : PaymentDto(0, OrderId, Amount, PaymentMethod, "Pending", null), IRequest<Response<string>>;
}
