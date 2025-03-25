using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderService.Api.Controllers;
using OrderService.Application.Commands;
using OrderService.Application.Queries;
using OrderService.Domain.Dtos;
using Shared.Bases;

namespace OrderService.Tests.Controllers
{
    public class OrdersControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly OrdersController _controller;

        public OrdersControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new OrdersController(_mediatorMock.Object);
        }

        private Response<T> CreateResponse<T>(T data, bool succeeded = true, string message = "")
        {
            return new Response<T> { Data = data, Succeeded = succeeded, Message = message };
        }

        private List<OrderItemDto> SampleItems => new List<OrderItemDto>
        {
            new OrderItemDto(1, 2, 10.00m),
            new OrderItemDto(2, 1, 20.00m)
        };

        [Fact]
        public async Task CreateOrder_ValidCommand_ReturnsCreated()
        {

            // Arrange
            var command = new CreateOrderCommand(DateTime.UtcNow, 100.50m, "Pending", SampleItems);
            var responseData = "1";
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(responseData, true, $"Order Created Successfully"));

            // Act
            var result = await _controller.CreateOrder(command);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult!.ActionName.Should().Be(nameof(_controller.CreateOrder));
            var response = createdResult!.Value as Response<string>;
            response!.Succeeded.Should().BeTrue();
            response.Message.Should().Be("Order Created Successfully");
        }

        [Fact]
        public async Task CreateOrder_InvalidCommand_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateOrderCommand(DateTime.UtcNow, -10m, "Pending", SampleItems);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<string>(null!, false, "Invalid order amount"));

            // Act
            var result = await _controller.CreateOrder(command);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            var response = createdResult!.Value as Response<string>;
            response!.Succeeded.Should().BeFalse();
            response.Message.Should().Be("Invalid order amount");
        }

        [Fact]
        public async Task UpdateOrder_ValidCommand_ReturnsOk()
        {
            // Arrange
            var command = new UpdateOrderCommand(1, DateTime.UtcNow, 150.75m, "Processing", SampleItems);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse("1", true, "Order Updated Successfully"));

            // Act
            var result = await _controller.UpdateOrder(1, command);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<string>;
            response!.Data.Should().Be("1");
            response.Succeeded.Should().BeTrue();
            response.Message.Should().Be("Order Updated Successfully");
        }

        [Fact]
        public async Task UpdateOrder_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var command = new UpdateOrderCommand(2, DateTime.UtcNow, 150.75m, "Processing", SampleItems);

            // Act
            var result = await _controller.UpdateOrder(1, command);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.Value.Should().Be("ID mismatch between route and command");
        }

        [Fact]
        public async Task UpdateOrder_NonExistingId_ReturnsBadRequest()
        {
            // Arrange
            var command = new UpdateOrderCommand(999, DateTime.UtcNow, 150.75m, "Processing", SampleItems);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<string>(null!, false, "Order not found"));

            // Act
            var result = await _controller.UpdateOrder(999, command);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            var response = badRequestResult!.Value as Response<string>;
            response!.Data.Should().BeNull();
            response.Succeeded.Should().BeFalse();
            response.Message.Should().Be("Order not found");
        }

        [Fact]
        public async Task DeleteOrder_ExistingId_ReturnsOk()
        {
            // Arrange
            var command = new DeleteOrderCommand(1);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse("1", true, "Order Deleted Successfully"));

            // Act
            var result = await _controller.DeleteOrder(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<string>;
            response!.Data.Should().Be("1");
            response.Succeeded.Should().BeTrue();
            response.Message.Should().Be("Order Deleted Successfully");
        }

        [Fact]
        public async Task DeleteOrder_NonExistingId_ReturnsBadRequest()
        {
            // Arrange
            var command = new DeleteOrderCommand(999);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<string>(null!, false, "Order not found"));

            // Act
            var result = await _controller.DeleteOrder(999);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            var response = badRequestResult!.Value as Response<string>;
            response!.Data.Should().BeNull();
            response.Succeeded.Should().BeFalse();
            response.Message.Should().Be("Order not found");
        }

        [Fact]
        public async Task GetAllOrders_ReturnsOkWithList()
        {
            // Arrange
            var orders = new List<OrderDto>
            {
                new OrderDto(1, DateTime.UtcNow, 100.50m, "Pending", SampleItems),
                new OrderDto(2, DateTime.UtcNow.AddDays(-1), 75.25m, "Completed", new List<OrderItemDto> { new OrderItemDto(3, 1, 75.25m) })
            };
            var query = new GetAllOrdersQuery();
            _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(orders, true));

            // Act
            var result = await _controller.GetAllOrders();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<List<OrderDto>>;
            response!.Data.Should().HaveCount(2);
            response.Data.Should().BeEquivalentTo(orders);
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task GetAllOrders_EmptyList_ReturnsOk()
        {
            // Arrange
            var query = new GetAllOrdersQuery();
            _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(new List<OrderDto>(), true));

            // Act
            var result = await _controller.GetAllOrders();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<List<OrderDto>>;
            response!.Data.Should().BeEmpty();
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task GetOrderById_ExistingId_ReturnsOk()
        {
            // Arrange
            var orderDto = new OrderDto(1, DateTime.UtcNow, 100.50m, "Pending", SampleItems);
            var query = new GetOrderByIdQuery(1);
            _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(orderDto, true));

            // Act
            var result = await _controller.GetOrderById(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<OrderDto>;
            response!.Data.Should().BeEquivalentTo(orderDto);
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task GetOrderById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var query = new GetOrderByIdQuery(999);
            _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<OrderDto>(null!, false, "Order not found"));

            // Act
            var result = await _controller.GetOrderById(999);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            var response = notFoundResult!.Value as Response<OrderDto>;
            response!.Data.Should().BeNull();
            response.Succeeded.Should().BeFalse();
            response.Message.Should().Be("Order not found");
        }
    }

}