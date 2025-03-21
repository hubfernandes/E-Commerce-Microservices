using MediatR;
using Payment.Domain.Dtos;
using Shared.Bases;

namespace Payment.Application.Queries
{
    public record GetPaymentByIdQuery(int Id) : IRequest<Response<PaymentDto>>;
}
