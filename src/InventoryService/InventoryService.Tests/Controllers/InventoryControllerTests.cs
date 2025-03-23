using FluentAssertions;
using InventoryService.Api.Controllers;
using InventoryService.Application.Commands;
using InventoryService.Application.Queries;
using InventoryService.Domain.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Bases;

namespace InventoryService.Tests.Controllers
{
    public class InventoryControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly InventoryController _controller;

        public InventoryControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new InventoryController(_mediatorMock.Object);
        }

        private Response<T> CreateResponse<T>(T data, bool succeeded = true, string message = "")
        {
            return new Response<T> { Data = data, Succeeded = succeeded, Message = message };
        }

        [Fact]
        public async Task GetStock_ValidProductId_ReturnsOk()
        {
            // Arrange
            var stockDto = new StockDto("1", 100, 0, "InStock");
            var query = new GetStockByProductIdQuery(1);
            _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(stockDto, true));

            // Act
            var result = await _controller.GetStock(1);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            var response = okResult!.Value as Response<StockDto>;
            response!.Data.Should().BeEquivalentTo(stockDto);
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task ReserveStock_ValidCommand_ReturnsOk()
        {
            // Arrange
            var command = new ReserveStockCommand(1, 5);
            var responseData = "Stock reserved successfully";
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(responseData, true));

            // Act
            var result = await _controller.ReserveStock(command);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<string>;
            response!.Data.Should().Be(responseData);
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task ReserveStock_InsufficientStock_ReturnsBadRequest()
        {
            // Arrange
            var command = new ReserveStockCommand(1, 1000);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<string>(null!, false, "Insufficient stock"));

            // Act
            var result = await _controller.ReserveStock(command);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            var response = badRequestResult!.Value as Response<string>;
            response!.Data.Should().BeNull();
            response.Succeeded.Should().BeFalse();
            response.Message.Should().Be("Insufficient stock");
        }

        [Fact]
        public async Task ReleaseStock_ValidCommand_ReturnsOk()
        {
            // Arrange
            var command = new ReleaseStockCommand(1, 5);
            var responseData = "Stock released successfully";
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(responseData, true));

            // Act
            var result = await _controller.ReleaseStock(command);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<string>;
            response!.Data.Should().Be(responseData);
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task ReleaseStock_InsufficientReservedStock_ReturnsBadRequest()
        {
            // Arrange
            var command = new ReleaseStockCommand(1, 1000);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<string>(null!, false, "Insufficient reserved stock"));

            // Act
            var result = await _controller.ReleaseStock(command);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            var response = badRequestResult!.Value as Response<string>;
            response!.Data.Should().BeNull();
            response.Succeeded.Should().BeFalse();
            response.Message.Should().Be("Insufficient reserved stock");
        }

        [Fact]
        public async Task UpdateStock_ValidCommand_ReturnsOk()
        {
            // Arrange
            var command = new UpdateStockCommand(1, 50);
            var responseData = "Stock updated successfully";
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(responseData, true));

            // Act
            var result = await _controller.UpdateStock(command);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<string>;
            response!.Data.Should().Be(responseData);
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task GetLowStock_ValidThreshold_ReturnsOk()
        {
            // Arrange
            var lowStockItems = new List<LowStockDto>
            {
                new LowStockDto(1, 5, 10),
                new LowStockDto(2, 8, 10)
            };
            var query = new GetLowStockQuery(10);
            _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(lowStockItems, true));

            // Act
            var result = await _controller.GetLowStock(10);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            var response = okResult!.Value as Response<List<LowStockDto>>;
            response!.Data.Should().HaveCount(2);
            response.Data.Should().BeEquivalentTo(lowStockItems);
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task GetLowStock_NoItemsBelowThreshold_ReturnsOkWithEmptyList()
        {
            // Arrange
            var query = new GetLowStockQuery(5);
            _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(new List<LowStockDto>(), true));

            // Act
            var result = await _controller.GetLowStock(5);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            var response = okResult!.Value as Response<List<LowStockDto>>;
            response!.Data.Should().BeEmpty();
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task ReconcileStock_ValidCommand_ReturnsOk()
        {
            // Arrange
            var command = new ReconcileStockCommand(1, 100, "Annual inventory check");
            var responseData = "Stock reconciled successfully";
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(responseData, true));

            // Act
            var result = await _controller.ReconcileStock(command);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<string>;
            response!.Data.Should().Be(responseData);
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task ReconcileStock_InvalidProductId_ReturnsBadRequest()
        {
            // Arrange
            var command = new ReconcileStockCommand(999, 100, "Annual inventory check");
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<string>(null!, false, "Product not found"));

            // Act
            var result = await _controller.ReconcileStock(command);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            var response = badRequestResult!.Value as Response<string>;
            response!.Data.Should().BeNull();
            response.Succeeded.Should().BeFalse();
            response.Message.Should().Be("Product not found");
        }
    }
}