using AutoMapper;
using MediatR;
using Payment.Application.Commands;
using Payment.Domain.Entities;
using Payment.Infrastructure.Interfaces;
using Shared.Bases;

namespace Payment.Application.Handlers
{
    public class PaymentCommandHandler : IRequestHandler<CreatePaymentCommand, Response<string>>,
                                         IRequestHandler<UpdatePaymentStatusCommand, Response<string>>

    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;
        private readonly ResponseHandler _responseHandler;

        public PaymentCommandHandler(
            IPaymentRepository paymentRepository,
            IMapper mapper,
            ResponseHandler responseHandler)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
            _responseHandler = responseHandler;
        }

        public async Task<Response<string>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {

            var payment = _mapper.Map<Domain.Entities.Payment>(request);
            var paymentResult = await ProcessPayment(payment);
            if (!paymentResult.IsSuccess)
            {
                payment.UpdateStatus(PaymentStatus.Failed, null!);
                await _paymentRepository.AddAsync(payment);
                return _responseHandler.BadRequest<string>("Payment processing failed");
            }

            payment.UpdateStatus(PaymentStatus.Completed, paymentResult.TransactionId);
            await _paymentRepository.AddAsync(payment);
            return _responseHandler.Created<string>($"Payment for Order {request.OrderId} processed successfully");
        }

        public async Task<Response<string>> Handle(UpdatePaymentStatusCommand request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByIdAsync(request.Id);
            if (payment == null)
                return _responseHandler.NotFound<string>("Payment not found");

            payment.UpdateStatus(PaymentStatus.Completed, request.TransactionId!);
            await _paymentRepository.UpdateAsync(payment);
            return _responseHandler.Success<string>("Payment status updated successfully");
        }

        public async Task<(bool IsSuccess, string TransactionId)> ProcessPayment(Domain.Entities.Payment payment)
        {
            await Task.Delay(100);
            return (true, Guid.NewGuid().ToString());
        }
    }
}
