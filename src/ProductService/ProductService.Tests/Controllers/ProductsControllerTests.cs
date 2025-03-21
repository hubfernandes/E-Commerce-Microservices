using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductService.Api.Controllers;
using ProductService.Application.Commands;
using ProductService.Application.Queries;
using ProductService.Domain.Dtos;
using Shared.Bases;

namespace ProductService.Tests.Controllers
{
    public class ProductsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new ProductsController(_mediatorMock.Object);
        }
        private Response<T> CreateResponse<T>(T data, bool succeeded = true, string message = "")
        {
            return new Response<T> { Data = data, Succeeded = succeeded, Message = message };
        }
        [Fact]
        public async Task GetProduct_ShouldReturn_Ok_WhenProductExists()
        {
            // Arrange
            var productDto = new ProductDto(1, "Test Product", 10.99m, 100, true);

            _mediatorMock.Setup(m => m.Send(It.Is<GetProductByIdQuery>(q => q.Id == 1), It.IsAny<CancellationToken>()))
             .ReturnsAsync(CreateResponse(productDto));


            // Act
            var result = await _controller.GetProduct(1);
            /*
             Without this cast, result is still just an IActionResult, and you cannot directly access properties of NotFoundObjectResult (like Value or StatusCode).
             */

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            // okResult.StatusCode.Should().Be(200);
            var response = okResult.Value as Response<ProductDto>;
            response!.Data.Should().BeEquivalentTo(productDto);
        }

        [Fact]
        public async Task GetProduct_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.Is<GetProductByIdQuery>(q => q.Id == 999), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<ProductDto>(null!, false, "Product not Found"));

            // Act
            var result = await _controller.GetProduct(999);
            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            var response = notFoundResult!.Value as Response<ProductDto>;
            response!.Data.Should().BeNull();
            response.Succeeded.Should().BeFalse();
            response.Message.Should().Be("Product not Found");
        }

        [Fact]
        public async Task GetAllProducts_ReturnsOkWithList()
        {
            // Arrange
            var products = new List<ProductDto>
        {
            new ProductDto(1, "Product 1", 5.99m, 50, true),
            new ProductDto(2, "Product 2", 15.49m, 25, false)
        };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(products));

            // Act
            var result = await _controller.GetAllProducts();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<List<ProductDto>>;
            response!.Data.Should().HaveCount(2);
            response.Data.Should().BeEquivalentTo(products);
            response.Succeeded.Should().BeTrue();
        }


        [Fact]
        public async Task GetAllProducts_EmptyList_ReturnsOk()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(new List<ProductDto>()));

            // Act
            var result = await _controller.GetAllProducts();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<List<ProductDto>>;
            response!.Data.Should().BeEmpty();
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task CreateProduct_ValidCommand_ReturnsCreated()
        {
            // Arrange
            var command = new CreateProductCommand(1, "New Product", 10.99m, 10, true);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse("1", true, "Product Created Successfully"));

            // Act
            var result = await _controller.CreateProduct(command);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            var response = createdResult!.Value as Response<string>;
            response!.Message.Should().Be("Product Created Successfully");
        }

        [Fact]
        public async Task UpdateProduct_ValidCommand_ReturnsOk()
        {
            // Arrange
            var command = new UpdateProductCommand(1, "Updated Product", 15.99m, 10, true);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse("1", true, "Product Updated Successfully"));

            // Act
            var result = await _controller.UpdateProduct(command);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<string>;
            response!.Data.Should().Be("1");
            response.Message.Should().Be("Product Updated Successfully");
        }

        [Fact]
        public async Task UpdateProduct_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var command = new UpdateProductCommand(999, "Non-existent", 12, 10, true);

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<string>(null!, false, "Product not found"));

            // Act
            var result = await _controller.UpdateProduct(command);

            // Assert                
            var okResult = result as ObjectResult;
            var response = okResult!.Value as Response<string>;
            response!.Data.Should().BeNull();
            response.Message.Should().Be("Product not found");
        }

        [Fact]
        public async Task DeleteProduct_ExistingId_ReturnsOk()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.Is<DeleteProductCommand>(c => c.Id == 1), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse("1", true, "Product Deleted Successfully"));

            // Act
            var result = await _controller.DeleteProduct(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<string>;
            response!.Succeeded.Should().BeTrue();
            response.Message.Should().Be("Product Deleted Successfully");
            response.Data.Should().Be("1");
        }

        [Fact]
        public async Task DeleteProduct_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.Is<DeleteProductCommand>(c => c.Id == 999), It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<string>(null!, false, "Product not found"));

            // Act
            var result = await _controller.DeleteProduct(999);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            var response = notFoundResult!.Value as Response<string>;
            response!.Succeeded.Should().BeFalse();
            response.Message.Should().Be("Product not found");
            response.Data.Should().BeNull();
        }
    }
}
