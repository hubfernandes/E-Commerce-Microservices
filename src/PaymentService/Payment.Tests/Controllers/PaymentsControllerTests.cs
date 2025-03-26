using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Payment.Api.Controllers;
using Payment.Application.Commands;
using Payment.Application.Queries;
using Payment.Domain.Dtos;
using Payment.Domain.Entities;
using Shared.Bases;

namespace Payment.Tests.Controllers
{
    public class PaymentsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly PaymentsController _controller;

        public PaymentsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new PaymentsController(_mediatorMock.Object);
        }

        private Response<T> CreateResponse<T>(T data, bool succeeded = true, string message = "")
        {
            return new Response<T> { Data = data, Succeeded = succeeded, Message = message };
        }

        [Fact]
        public async Task GetPaymentById_ShouldReturn_Ok_WhenPaymentExists()
        {
            // Arrange
            var paymentDto = new PaymentDto(1, 1, 99.99m, "CreditCard", "Completed", "TXN-123456789");
            _mediatorMock.Setup(m => m.Send(It.Is<GetPaymentByIdQuery>(q => q.Id == 1), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(paymentDto));

            // Act
            var result = await _controller.GetPaymentById(1);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value as Response<PaymentDto>;
            response!.Data.Should().BeEquivalentTo(paymentDto);
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task GetPaymentById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.Is<GetPaymentByIdQuery>(q => q.Id == 999), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<PaymentDto>(null!, false, "Payment not found"));

            // Act
            var result = await _controller.GetPaymentById(999);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            var response = notFoundResult!.Value as Response<PaymentDto>;
            response!.Data.Should().BeNull();
            response.Succeeded.Should().BeFalse();
            response.Message.Should().Be("Payment not found");
        }

        [Fact]
        public async Task GetPaymentsByOrderId_ReturnsOkWithList()
        {
            // Arrange
            var payments = new List<PaymentDto>
            {
                new PaymentDto(1, 1, 99.99m, "CreditCard", "Completed", "TXN-123456789"),
                new PaymentDto(2, 1, 49.50m, "PayPal", "Pending", null)
            };
            _mediatorMock.Setup(m => m.Send(It.Is<GetPaymentsByOrderIdQuery>(q => q.OrderId == 1), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(payments));

            // Act
            var result = await _controller.GetPaymentsByOrderId(1);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value as Response<List<PaymentDto>>;
            response!.Data.Should().HaveCount(2);
            response.Data.Should().BeEquivalentTo(payments);
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task GetPaymentsByOrderId_NoPayments_ReturnsNotFound()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.Is<GetPaymentsByOrderIdQuery>(q => q.OrderId == 999), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<List<PaymentDto>>(null!, false, "No payments found for this order"));

            // Act
            var result = await _controller.GetPaymentsByOrderId(999);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            var response = notFoundResult!.Value as Response<List<PaymentDto>>;
            response!.Data.Should().BeNull();
            response.Succeeded.Should().BeFalse();
            response.Message.Should().Be("No payments found for this order");
        }

        [Fact]
        public async Task CreatePayment_ValidCommand_ReturnsOk()
        {
            // Arrange
            var command = new CreatePaymentCommand(1, 99.99m, PaymentStatus.Completed, PaymentMethodType.CreditCard);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse("Payment processed", true, "Payment for Order 1 processed successfully"));

            // Act
            var result = await _controller.CreatePayment(command);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value as Response<string>;
            response!.Data.Should().Be("Payment processed");
            response.Message.Should().Be("Payment for Order 1 processed successfully");
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task CreatePayment_InvalidCommand_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreatePaymentCommand(999, 0m, PaymentStatus.Failed, 0);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<string>(null!, false, "Payment processing failed"));

            // Act
            var result = await _controller.CreatePayment(command);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var response = badRequestResult.Value as Response<string>;
            response!.Data.Should().BeNull();
            response.Succeeded.Should().BeFalse();
            response.Message.Should().Be("Payment processing failed");
        }

        [Fact]
        public async Task UpdatePaymentStatus_ValidCommand_ReturnsOk()
        {
            // Arrange
            var command = new UpdatePaymentStatusCommand(1, "Completed", "TXN-987654321");
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse("Payment updated", true, "Payment status updated successfully"));

            // Act
            var result = await _controller.UpdatePaymentStatus(1, command);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value as Response<string>;
            response!.Data.Should().Be("Payment updated");
            response.Message.Should().Be("Payment status updated successfully");
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task UpdatePaymentStatus_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var command = new UpdatePaymentStatusCommand(2, "Completed", "TXN-987654321");

            // Act
            var result = await _controller.UpdatePaymentStatus(1, command);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.Value.Should().Be("ID mismatch between route and command");
        }

        [Fact]
        public async Task UpdatePaymentStatus_NonExistingId_ReturnsBadRequest()
        {
            // Arrange
            var command = new UpdatePaymentStatusCommand(999, "Completed", "TXN-987654321");
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<string>(null!, false, "Payment not found"));

            // Act
            var result = await _controller.UpdatePaymentStatus(999, command);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var response = badRequestResult.Value as Response<string>;
            response!.Data.Should().BeNull();
            response.Succeeded.Should().BeFalse();
            response.Message.Should().Be("Payment not found");
        }
    }
}
