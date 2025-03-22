using CartService.Api.Controllers;
using CartService.Application.Commands;
using CartService.Application.Queries;
using CartService.Domain.Dtos;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Bases;

namespace CartService.Tests.Controllers
{
    public class CartsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CartsController _controller;

        public CartsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new CartsController(_mediatorMock.Object);
        }

        private Response<T> CreateResponse<T>(T data, bool succeeded = true, string message = "")
        {
            return new Response<T> { Data = data, Succeeded = succeeded, Message = message };
        }

        private List<CartItemDto> SampleItems => new List<CartItemDto>
        {
            new CartItemDto(101, 2, 19.99m),
            new CartItemDto(102, 1, 49.50m)
        };

        [Fact]
        public async Task CreateCart_ValidCommand_ReturnsCreated()
        {
            // Arrange
            var command = new CreateCartCommand("user1", SampleItems);
            var responseData = "Cart created";
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(responseData, true, "Cart Created Successfully"));

            // Act
            var result = await _controller.CreateCart(command);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult!.ActionName.Should().Be(nameof(_controller.CreateCart));
            var response = createdResult!.Value as Response<string>;
            response!.Succeeded.Should().BeTrue();
            response.Message.Should().Be("Cart Created Successfully");
        }

        [Fact]
        public async Task CreateCart_InvalidCommand_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateCartCommand("", SampleItems);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<string>(null!, false, "User ID is required"));

            // Act
            var result = await _controller.CreateCart(command);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            var response = badRequestResult!.Value as Response<string>;
            response!.Succeeded.Should().BeFalse();
            response.Message.Should().Be("User ID is required");
        }

        [Fact]
        public async Task UpdateCart_ValidCommand_ReturnsOk()
        {
            // Arrange
            var command = new UpdateCartCommand(1, "user1", SampleItems);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse("1", true, "Cart Updated Successfully"));

            // Act
            var result = await _controller.UpdateCart(1, command);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<string>;
            response!.Data.Should().Be("1");
            response.Succeeded.Should().BeTrue();
            response.Message.Should().Be("Cart Updated Successfully");
        }

        [Fact]
        public async Task UpdateCart_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var command = new UpdateCartCommand(2, "user1", SampleItems);

            // Act
            var result = await _controller.UpdateCart(1, command);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.Value.Should().Be("ID mismatch between route and command");
        }

        [Fact]
        public async Task UpdateCart_NonExistingId_ReturnsBadRequest()
        {
            // Arrange
            var command = new UpdateCartCommand(999, "user1", SampleItems);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<string>(null!, false, "Cart not found"));

            // Act
            var result = await _controller.UpdateCart(999, command);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            var response = badRequestResult!.Value as Response<string>;
            response!.Data.Should().BeNull();
            response.Succeeded.Should().BeFalse();
            response.Message.Should().Be("Cart not found");
        }

        [Fact]
        public async Task DeleteCart_ExistingId_ReturnsOk()
        {
            // Arrange
            var command = new DeleteCartCommand(1);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse("1", true, "Cart Deleted Successfully"));

            // Act
            var result = await _controller.DeleteCart(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<string>;
            response!.Data.Should().Be("1");
            response.Succeeded.Should().BeTrue();
            response.Message.Should().Be("Cart Deleted Successfully");
        }

        [Fact]
        public async Task DeleteCart_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var command = new DeleteCartCommand(999);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<string>(null!, false, "Cart not found"));

            // Act
            var result = await _controller.DeleteCart(999);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            var response = notFoundResult!.Value as Response<string>;
            response!.Data.Should().BeNull();
            response.Succeeded.Should().BeFalse();
            response.Message.Should().Be("Cart not found");
        }

        [Fact]
        public async Task GetAllCarts_ReturnsOkWithList()
        {
            // Arrange
            var carts = new List<CartDto>
            {
                new CartDto(1, "user1", SampleItems),
                new CartDto(2, "user2", new List<CartItemDto> { new CartItemDto(103, 3, 10.00m) })
            };
            var query = new GetAllCartsQuery();
            _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(carts, true));

            // Act
            var result = await _controller.GetAllCarts();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<List<CartDto>>;
            response!.Data.Should().HaveCount(2);
            response.Data.Should().BeEquivalentTo(carts);
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task GetAllCarts_EmptyList_ReturnsOk()
        {
            // Arrange
            var query = new GetAllCartsQuery();
            _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(new List<CartDto>(), true));

            // Act
            var result = await _controller.GetAllCarts();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<List<CartDto>>;
            response!.Data.Should().BeEmpty();
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task GetCartById_ExistingId_ReturnsOk()
        {
            // Arrange
            var cartDto = new CartDto(1, "user1", SampleItems);
            var query = new GetCartByIdQuery(1);
            _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(cartDto, true));

            // Act
            var result = await _controller.GetCartById(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<CartDto>;
            response!.Data.Should().BeEquivalentTo(cartDto);
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task GetCartById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var query = new GetCartByIdQuery(999);
            _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<CartDto>(null!, false, "Cart not found"));

            // Act
            var result = await _controller.GetCartById(999);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            var response = notFoundResult!.Value as Response<CartDto>;
            response!.Data.Should().BeNull();
            response.Succeeded.Should().BeFalse();
            response.Message.Should().Be("Cart not found");
        }
    }
}
