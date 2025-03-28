using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Bases;
using WishlistService.Api.Controllers;
using WishlistService.Application.Commands;
using WishlistService.Application.Queries;
using WishlistService.Domain.Dtos;

namespace WishlistService.Tests.Controllers
{
    public class WishlistsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly WishlistsController _controller;

        public WishlistsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new WishlistsController(_mediatorMock.Object);
        }

        private Response<T> CreateResponse<T>(T data, bool succeeded = true, string message = "")
        {
            return new Response<T> { Data = data, Succeeded = succeeded, Message = message };
        }

        private List<WishlistItemDto> SampleItems => new List<WishlistItemDto>
        {
            new WishlistItemDto { ProductId = 101 },
            new WishlistItemDto { ProductId = 102 }
        };

        [Fact]
        public async Task CreateWishlist_ValidCommand_ReturnsCreated()
        {
            // Arrange
            var command = new CreateWishlistCommand("user1", SampleItems);
            var responseData = "Wishlist created";
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(responseData, true, "Wishlist created successfully"));

            // Act
            var result = await _controller.CreateWishlist(command);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult!.ActionName.Should().Be(nameof(_controller.GetWishlistById));
            var response = createdResult!.Value as Response<string>;
            response!.Succeeded.Should().BeTrue();
            response.Message.Should().Be("Wishlist created successfully");
        }

        [Fact]
        public async Task CreateWishlist_InvalidCommand_ReturnsBadRequest()
        {
            // Arrange
            var command = new CreateWishlistCommand("", SampleItems);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<string>(null!, false, "User ID is required"));

            // Act
            var result = await _controller.CreateWishlist(command);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            var response = badRequestResult!.Value as Response<string>;
            response!.Succeeded.Should().BeFalse();
            response.Message.Should().Be("User ID is required");
        }

        [Fact]
        public async Task UpdateWishlist_ValidCommand_ReturnsOk()
        {
            // Arrange
            var command = new UpdateWishlistCommand(1, "user1", SampleItems);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse("Wishlist updated", true, "Wishlist updated successfully"));

            // Act
            var result = await _controller.UpdateWishlist(1, command);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<string>;
            response!.Data.Should().Be("Wishlist updated");
            response.Succeeded.Should().BeTrue();
            response.Message.Should().Be("Wishlist updated successfully");
        }

        [Fact]
        public async Task UpdateWishlist_IdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var command = new UpdateWishlistCommand(2, "user1", SampleItems);

            // Act
            var result = await _controller.UpdateWishlist(1, command);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.Value.Should().Be("ID mismatch");
        }

        [Fact]
        public async Task UpdateWishlist_NonExistingId_ReturnsBadRequest()
        {
            // Arrange
            var command = new UpdateWishlistCommand(999, "user1", SampleItems);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<string>(null!, false, "Wishlist not found"));

            // Act
            var result = await _controller.UpdateWishlist(999, command);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            var response = badRequestResult!.Value as Response<string>;
            response!.Data.Should().BeNull();
            response.Succeeded.Should().BeFalse();
            response.Message.Should().Be("Wishlist not found");
        }

        [Fact]
        public async Task DeleteWishlist_ExistingId_ReturnsOk()
        {
            // Arrange
            var command = new DeleteWishlistCommand(1);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse("Wishlist deleted", true, "Wishlist deleted successfully"));

            // Act
            var result = await _controller.DeleteWishlist(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<string>;
            response!.Data.Should().Be("Wishlist deleted");
            response.Succeeded.Should().BeTrue();
            response.Message.Should().Be("Wishlist deleted successfully");
        }

        [Fact]
        public async Task DeleteWishlist_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var command = new DeleteWishlistCommand(999);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<string>(null!, false, "Wishlist not found"));

            // Act
            var result = await _controller.DeleteWishlist(999);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            var response = notFoundResult!.Value as Response<string>;
            response!.Data.Should().BeNull();
            response.Succeeded.Should().BeFalse();
            response.Message.Should().Be("Wishlist not found");
        }

        [Fact]
        public async Task DeleteWishlistByUserId_ExistingUserId_ReturnsOk()
        {
            // Arrange
            var command = new DeleteWishlistByUserIdCommand("user1");
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse("Wishlist deleted", true, "Wishlist deleted successfully"));

            // Act
            var result = await _controller.DeleteWishlistByUserId("user1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<string>;
            response!.Data.Should().Be("Wishlist deleted");
            response.Succeeded.Should().BeTrue();
            response.Message.Should().Be("Wishlist deleted successfully");
        }

        [Fact]
        public async Task DeleteWishlistByUserId_NonExistingUserId_ReturnsNotFound()
        {
            // Arrange
            var command = new DeleteWishlistByUserIdCommand("user999");
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<string>(null!, false, "Wishlist not found for user"));

            // Act
            var result = await _controller.DeleteWishlistByUserId("user999");

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            var response = notFoundResult!.Value as Response<string>;
            response!.Data.Should().BeNull();
            response.Succeeded.Should().BeFalse();
            response.Message.Should().Be("Wishlist not found for user");
        }

        [Fact]
        public async Task GetAllWishlists_ReturnsOkWithList()
        {
            // Arrange
            var wishlists = new List<WishlistDto>
            {
                new WishlistDto { Id = 1, UserId = "user1", Items = SampleItems },
                new WishlistDto { Id = 2, UserId = "user2", Items = new List<WishlistItemDto> { new() { ProductId = 103 } } }
            };
            var query = new GetAllWishlistsQuery();
            _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(wishlists, true));

            // Act
            var result = await _controller.GetAllWishlists();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<List<WishlistDto>>;
            response!.Data.Should().HaveCount(2);
            response.Data.Should().BeEquivalentTo(wishlists);
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task GetAllWishlists_EmptyList_ReturnsOk()
        {
            // Arrange
            var query = new GetAllWishlistsQuery();
            _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(new List<WishlistDto>(), true));

            // Act
            var result = await _controller.GetAllWishlists();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<List<WishlistDto>>;
            response!.Data.Should().BeEmpty();
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task GetWishlistById_ExistingId_ReturnsOk()
        {
            // Arrange
            var wishlistDto = new WishlistDto { Id = 1, UserId = "user1", Items = SampleItems };
            var query = new GetWishlistByIdQuery(1);
            _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(wishlistDto, true));

            // Act
            var result = await _controller.GetWishlistById(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<WishlistDto>;
            response!.Data.Should().BeEquivalentTo(wishlistDto);
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task GetWishlistById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var query = new GetWishlistByIdQuery(999);
            _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<WishlistDto>(null!, false, "Wishlist not found"));

            // Act
            var result = await _controller.GetWishlistById(999);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            var response = notFoundResult!.Value as Response<WishlistDto>;
            response!.Data.Should().BeNull();
            response.Succeeded.Should().BeFalse();
            response.Message.Should().Be("Wishlist not found");
        }

        [Fact]
        public async Task GetWishlistByUserId_ExistingUserId_ReturnsOk()
        {
            // Arrange
            var wishlistDto = new WishlistDto { Id = 1, UserId = "user1", Items = SampleItems };
            var query = new GetWishlistByUserIdQuery("user1");
            _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(wishlistDto, true));

            // Act
            var result = await _controller.GetWishlistByUserId("user1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<WishlistDto>;
            response!.Data.Should().BeEquivalentTo(wishlistDto);
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task GetWishlistByUserId_NonExistingUserId_ReturnsNotFound()
        {
            // Arrange
            var query = new GetWishlistByUserIdQuery("user999");
            _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<WishlistDto>(null!, false, "Wishlist not found for user"));

            // Act
            var result = await _controller.GetWishlistByUserId("user999");

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            var response = notFoundResult!.Value as Response<WishlistDto>;
            response!.Data.Should().BeNull();
            response.Succeeded.Should().BeFalse();
            response.Message.Should().Be("Wishlist not found for user");
        }

        [Fact]
        public async Task AddItemToWishlist_ValidCommand_ReturnsOk()
        {
            // Arrange
            var command = new AddItemToWishlistCommand("user1", 103);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse("Item added", true, "Item added to wishlist"));

            // Act
            var result = await _controller.AddItemToWishlist(command);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<string>;
            response!.Data.Should().Be("Item added");
            response.Succeeded.Should().BeTrue();
            response.Message.Should().Be("Item added to wishlist");
        }

        [Fact]
        public async Task AddItemToWishlist_InvalidCommand_ReturnsBadRequest()
        {
            // Arrange
            var command = new AddItemToWishlistCommand("", 103);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<string>(null!, false, "User ID is required"));

            // Act
            var result = await _controller.AddItemToWishlist(command);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            var response = badRequestResult!.Value as Response<string>;
            response!.Data.Should().BeNull();
            response.Succeeded.Should().BeFalse();
            response.Message.Should().Be("User ID is required");
        }

        [Fact]
        public async Task RemoveItemFromWishlist_ValidCommand_ReturnsOk()
        {
            // Arrange
            var command = new RemoveItemFromWishlistCommand("user1", 101);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse("Item removed", true, "Item removed from wishlist"));

            // Act
            var result = await _controller.RemoveItemFromWishlist(command);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<string>;
            response!.Data.Should().Be("Item removed");
            response.Succeeded.Should().BeTrue();
            response.Message.Should().Be("Item removed from wishlist");
        }

        [Fact]
        public async Task RemoveItemFromWishlist_NonExistingWishlist_ReturnsBadRequest()
        {
            // Arrange
            var command = new RemoveItemFromWishlistCommand("user999", 101);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<string>(null!, false, "Wishlist not found for user"));

            // Act
            var result = await _controller.RemoveItemFromWishlist(command);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            var response = badRequestResult!.Value as Response<string>;
            response!.Data.Should().BeNull();
            response.Succeeded.Should().BeFalse();
            response.Message.Should().Be("Wishlist not found for user");
        }
    }
}