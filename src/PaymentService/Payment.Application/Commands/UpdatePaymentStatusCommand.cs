using MediatR;
using Payment.Domain.Dtos;
using Shared.Bases;

namespace Payment.Application.Commands
{
    public record UpdatePaymentStatusCommand(int Id, string Status, string TransactionId)
         : PaymentDto(Id, 0, 0, null, Status, TransactionId), IRequest<Response<string>>;
}
