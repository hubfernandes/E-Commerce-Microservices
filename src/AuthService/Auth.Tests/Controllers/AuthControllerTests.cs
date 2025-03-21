// Auth.Tests/Controllers/AuthControllerTests.cs
using Auth.Api.Controllers;
using Auth.Application.Commands;
using Auth.Application.Queries;
using Auth.Domain.Dtos;
using Auth.Domain.Entities;
using Auth.Domain.Responses;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using Shared.Bases;
using Shared.Wrapper;

namespace Auth.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly AccountController _controller;

        public AuthControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new AccountController(_mediatorMock.Object, null!);
        }

        private Response<T> CreateResponse<T>(T data, bool succeeded = true, string message = "")
        {
            return new Response<T> { Data = data, Succeeded = succeeded, Message = message };
        }

        private PaginatedResult<T> CreatePaginatedResult<T>(List<T> items, int totalCount, int pageNumber, int pageSize)
        {
            return PaginatedResult<T>.Success(items, totalCount, pageNumber, pageSize);
        }
        private AuthResponse DefaultAuthResponse => new AuthResponse
        {
            Token = "default_access_token",
            RefreshToken = "default_refresh_token",
            IsAuthenticated = true,
            Message = "Authentication successful",
            UserName = "defaultUser"
        };

        [Fact]
        public async Task ConfirmEmail_ValidCommand_ReturnsOk()
        {
            // Arrange
            var command = new ConfirmEmailCommand("user1", "validToken");
            var appUser = new AppUser { Id = "user1", UserName = "testuser", Email = "test@example.com" };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(appUser, true, "Email confirmed successfully"));

            // Act
            var result = await _controller.ConfirmEmail(command.UserId, command.Token);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<AppUser>;
            response!.Data.Should().BeEquivalentTo(appUser);
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task ConfirmEmail_InvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var command = new ConfirmEmailCommand("user1", "invalidToken");
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<AppUser>(null!, false, "Email confirmation failed"));

            // Act
            var result = await _controller.ConfirmEmail(command.UserId, command.Token);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            var response = badRequestResult!.Value as Response<AppUser>;
            response!.Data.Should().BeNull();
            response.Succeeded.Should().BeFalse();
            response.Message.Should().Be("Email confirmation failed");
        }

        [Fact]
        public async Task ChangePassword_ValidCommand_ReturnsOk()
        {
            // Arrange
            var command = new ChangePasswordCommand("oldPass", "newPass", "newPass");
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse("Password changed", true));

            // Act
            var result = await _controller.ChangePassword(command);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<string>;
            response!.Data.Should().Be("Password changed");
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteUser_ExistingId_ReturnsOk()
        {
            // Arrange
            var command = new DeleteAppUserCommand("user1");
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse("", true, "Account deleted successfully"));

            // Act
            var result = await _controller.DeleteUser(command.Id!);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<string>;
            response!.Succeeded.Should().BeTrue();
            response.Message.Should().Be("Account deleted successfully");
        }

        [Fact]
        public async Task ForgotPassword_ValidEmail_ReturnsOk()
        {
            // Arrange
            var command = new ForgotPasswordCommand("test@example.com");
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse("Password reset link sent successfully", true));

            // Act
            var result = await _controller.ForgotPassword(command);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<string>;
            response!.Succeeded.Should().BeTrue();
            response.Data.Should().Be("Password reset link sent successfully");
        }

        [Fact]
        public async Task LoginWithGoogle_ReturnsChallengeResult()
        {
            // Arrange
            var redirectUri = "http://localhost:5089/api/Account/Google-Login-Call-back";
            var authProperties = new AuthenticationProperties
            {
                RedirectUri = redirectUri
            };

            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(u => u.Action(It.Is<UrlActionContext>(ctx => ctx.Action == nameof(_controller.GoogleLoginCallback))))
                .Returns(redirectUri);
            _controller.Url = urlHelperMock.Object;

            _mediatorMock.Setup(m => m.Send(It.Is<GoogleLoginCommand>(cmd => cmd.RedirectUri == redirectUri), It.IsAny<CancellationToken>()))
                .ReturnsAsync(authProperties);

            // Act
            var result = await _controller.LoginWithGoogle();

            // Assert
            result.Should().BeOfType<ChallengeResult>();
            var challengeResult = result as ChallengeResult;

            challengeResult!.AuthenticationSchemes.Should().ContainSingle("Google");
            challengeResult.Properties.Should().Be(authProperties);
            challengeResult.Properties.RedirectUri.Should().Be(redirectUri);
        }

        // POST: api/auth/google-callback
        [Fact]
        public async Task GoogleLoginCallback_ReturnsOk()
        {
            // Arrange
            var command = new GoogleLoginCallbackCommand();
            var authResponse = DefaultAuthResponse;
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(authResponse);

            // Act
            var result = await _controller.GoogleLoginCallback();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().Be(authResponse);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            // Arrange
            var command = new LoginCommand("test@example.com", "password");
            var authResponse = DefaultAuthResponse;
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(authResponse, true));

            // Act
            var result = await _controller.Login(command);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<AuthResponse>;
            response!.Data.Should().BeEquivalentTo(authResponse);
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task RefreshToken_ValidTokens_ReturnsOk()
        {
            // Arrange
            var command = new RefreshTokenCommand("oldAccess", "oldRefresh");
            var authResponse = DefaultAuthResponse;
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(authResponse, true));

            // Act
            var result = await _controller.RefreshToken(command);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<AuthResponse>;
            response!.Data.Should().BeEquivalentTo(authResponse);
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task RegisterAdmin_ValidCommand_ReturnsCreated()
        {
            // Arrange
            var command = new RegisterAdminCommand("admin", "pass", "pass", "admin@example.com");
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse("user1", true));

            // Act
            var result = await _controller.Register_For_Admin(command);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult!.ActionName.Should().Be(nameof(_controller.Register_For_Admin));
            var response = createdResult.Value as Response<string>;
            response!.Data.Should().Be("user1");
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task RegisterUser_ValidCommand_ReturnsCreated()
        {
            // Arrange
            var command = new RegisterUserCommand("user", "pass", "pass", "user@example.com");
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse("user2", true));

            // Act
            var result = await _controller.Register_For_User(command);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult!.ActionName.Should().Be(nameof(_controller.Register_For_User));
            var response = createdResult.Value as Response<string>;
            response!.Data.Should().Be("user2");
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task ResetPassword_ValidCommand_ReturnsOk()
        {
            // Arrange
            var command = new ResetPasswordCommand("test@example.com", "token", "newPass", "newPass");
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse("Password reset", true));

            // Act
            var result = await _controller.ResetPassword(command);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<string>;
            response!.Data.Should().Be("Password reset");
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateUser_ValidCommand_ReturnsOk()
        {
            // Arrange
            var command = new UpdateAppUserCommand("user1", "newUser", "new@example.com", "1234567890");
            var userDto = new AppUserDto("user1", "newUser", "new@example.com", "1234567890");
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(userDto, true));

            // Act
            var result = await _controller.UpdateUser(command.Id!, command);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<AppUserDto>;
            response!.Data.Should().BeEquivalentTo(userDto);
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateProfile_ValidCommand_ReturnsOk()
        {
            // Arrange
            var innerCommand = new UpdateProfileCommand("newUser", "new@example.com", "1234567890");
            var command = new UpdateProfileRequest("user1", innerCommand);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse("Profile updated successfully", true));

            // Act
            var result = await _controller.UpdateProfile(innerCommand);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<string>;
            response!.Data.Should().Be("Profile updated successfully");
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOkWithList()
        {
            // Arrange
            var users = new List<AppUserDto>
            {
                new AppUserDto("user1", "user1", "user1@example.com", "123"),
                new AppUserDto("user2", "user2", "user2@example.com", "456")
            };
            var query = new GetAllAppUsers { PageNumber = 1, PageSize = 10 };
            _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreatePaginatedResult(users, 2, 1, 10));

            // Act
            var result = await _controller.GetUsers(query);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as PaginatedResult<AppUserDto>;
            response!.Data.Should().HaveCount(2);
            response.Data.Should().BeEquivalentTo(users);
        }

        [Fact]
        public async Task GetUserById_ExistingId_ReturnsOk()
        {
            // Arrange
            var userDto = new AppUserDto("user1", "testuser", "test@example.com", "1234567890");
            var query = new GetAppUserById("user1");
            _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse(userDto, true));

            // Act
            var result = await _controller.GetUserById("user1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as Response<AppUserDto>;
            response!.Data.Should().BeEquivalentTo(userDto);
            response.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task GetUserById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var query = new GetAppUserById("nonexistent");
            _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateResponse<AppUserDto>(null!, false, "Unauthorized"));

            // Act
            var result = await _controller.GetUserById("nonexistent");

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var unauthorizedResult = result as NotFoundObjectResult;
            var response = unauthorizedResult!.Value as Response<AppUserDto>;
            response!.Data.Should().BeNull();
            response.Succeeded.Should().BeFalse();
        }
    }
}