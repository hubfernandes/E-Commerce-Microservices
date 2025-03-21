using MediatR;
using Payment.Domain.Dtos;
using Shared.Bases;

namespace Payment.Application.Queries
{
    public record GetPaymentsByOrderIdQuery(int OrderId) : IRequest<Response<List<PaymentDto>>>;
}
