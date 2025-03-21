using AutoMapper;
using MediatR;
using Payment.Application.Commands;
using Payment.Infrastructure.Interfaces;
using Shared.Bases;

namespace Payment.Application.Handlers
{
    public class PaymentCommandHandler :
        IRequestHandler<CreatePaymentCommand, Response<string>>,
        IRequestHandler<UpdatePaymentStatusCommand, Response<string>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;
        private readonly ResponseHandler _responseHandler;
        private readonly HttpClient _httpClient;

        public PaymentCommandHandler(
            IPaymentRepository paymentRepository,
            IMapper mapper,
            ResponseHandler responseHandler,
            IHttpClientFactory httpClientFactory)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
            _responseHandler = responseHandler;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<Response<string>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            // Validate order exists by calling OrderService
            var orderResponse = await _httpClient.GetAsync($"http://orderservice/api/orders/{request.OrderId}");
            if (!orderResponse.IsSuccessStatusCode)
                return _responseHandler.BadRequest<string>("Order not found");

            var payment = _mapper.Map<Domain.Entities.Payment>(request);

            // Simulate payment gateway processing
            var paymentResult = await ProcessPayment(payment);
            if (!paymentResult.IsSuccess)
            {
                payment.UpdateStatus("Failed", null);
                await _paymentRepository.AddAsync(payment);
                return _responseHandler.BadRequest<string>("Payment processing failed");
            }

            payment.UpdateStatus("Completed", paymentResult.TransactionId);
            await _paymentRepository.AddAsync(payment);
            return _responseHandler.Created<string>($"Payment for Order {request.OrderId} processed successfully");
        }

        public async Task<Response<string>> Handle(UpdatePaymentStatusCommand request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByIdAsync(request.Id);
            if (payment == null)
                return _responseHandler.NotFound<string>("Payment not found");

            payment.UpdateStatus(request.Status, request.TransactionId);
            await _paymentRepository.UpdateAsync(payment);
            return _responseHandler.Success<string>("Payment status updated successfully");
        }

        // Mock payment gateway processing
        private async Task<(bool IsSuccess, string TransactionId)> ProcessPayment(Domain.Entities.Payment payment)
        {
            await Task.Delay(100); // Simulate network call
            return (true, Guid.NewGuid().ToString()); // Mock success
        }
    }
}
