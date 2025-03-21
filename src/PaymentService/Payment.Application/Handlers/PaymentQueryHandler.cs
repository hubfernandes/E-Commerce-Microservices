using AutoMapper;
using MediatR;
using Payment.Application.Queries;
using Payment.Domain.Dtos;
using Payment.Infrastructure.Interfaces;
using Shared.Bases;

namespace Payment.Application.Handlers
{
    public class PaymentQueryHandler :
         IRequestHandler<GetPaymentByIdQuery, Response<PaymentDto>>,
         IRequestHandler<GetPaymentsByOrderIdQuery, Response<List<PaymentDto>>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;
        private readonly ResponseHandler _responseHandler;

        public PaymentQueryHandler(
            IPaymentRepository paymentRepository,
            IMapper mapper,
            ResponseHandler responseHandler)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
            _responseHandler = responseHandler;
        }

        public async Task<Response<PaymentDto>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByIdAsync(request.Id);
            if (payment == null)
                return _responseHandler.NotFound<PaymentDto>("Payment not found");

            var mappedPayment = _mapper.Map<PaymentDto>(payment);
            return _responseHandler.Success(mappedPayment);
        }

        public async Task<Response<List<PaymentDto>>> Handle(GetPaymentsByOrderIdQuery request, CancellationToken cancellationToken)
        {
            var payments = await _paymentRepository.GetByOrderIdAsync(request.OrderId);
            if (payments == null || !payments.Any())
                return _responseHandler.NotFound<List<PaymentDto>>("No payments found for this order");

            var mappedPayments = _mapper.Map<List<PaymentDto>>(payments);
            return _responseHandler.Success(mappedPayments);
        }
    }
}
