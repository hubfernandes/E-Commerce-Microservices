using Auth.Application.Commands;
using Auth.Application.Queries;
using Auth.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Bases;
using System.Security.Claims;

namespace Auth.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public IMediator _mediator;
        public readonly ResponseHandler _responseHandler;

        public AccountController(IMediator mediator, ResponseHandler responseHandler)
        {
            _mediator = mediator;
            _responseHandler = responseHandler;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var command = new GetAppUserById(userId);
            var result = await _mediator.Send(command);
            return (bool)result.Succeeded! ? Ok(result) : NotFound(result);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("Users")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _mediator.Send(new GetAllAppUsers());
            return result.Succeeded ? Ok(result) : NotFound(result);
        }


        [HttpGet("login-With-Google")]
        public async Task<IActionResult> LoginWithGoogle()
        {
            var properties = await _mediator.Send(new GoogleLoginCommand(Url.Action(nameof(GoogleLoginCallback))));
            return Challenge(properties, "Google");
        }

        [HttpGet("Google-Login-Call-back")]
        public async Task<IActionResult> GoogleLoginCallback()
        {
            var token = await _mediator.Send(new GoogleLoginCallbackCommand());
            return Ok(new { Token = token });
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token) // from query as i fetch it from query params 
        {
            var result = await _mediator.Send(new ConfirmEmailCommand(userId, token));
            if (result.Succeeded == null) return _responseHandler.BadRequest<AppUser>(result.Message));

            return Ok(new { Message = result });
        }


        [HttpPost("register/admin")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            if (!ModelState.IsValid)
            {
                return NewResult(_responseHandler.BadRequest<AppUser>("Invalid request data."));
            }

            var response = await _mediator.Send(command); // It sends a command to a corresponding handler, which then executes the necessary business logic

            if (response.Succeeded == null)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand request)
        {
            var result = await _mediator.Send(request);
            return (bool)result.Succeeded! ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return (bool)result.Succeeded! ? Ok(result) : BadRequest(result);

        }


        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return NewResult(_responseHandler.Unauthorized<AppUser>());

            var result = await _mediator.Send(command);
            return (bool)result.Succeeded! ? Ok(result) : BadRequest(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return (bool)result.Succeeded! ? Ok(result) : BadRequest(result);

        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return (bool)result.Succeeded! ? Ok(result) : NotFound(result);

        }

        [Authorize(Policy = "admin")]
        [HttpPut("update-user/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateAppUserCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("User ID mismatch.");
            }

            var result = await _mediator.Send(command);
            return (bool)result.Succeeded! ? Ok(result) : BadRequest(result);

        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand command)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return NewResult(_responseHandler.Unauthorized<AppUser>());

            var result = await _mediator.Send(new UpdateProfileRequest(userId, command));
            return (bool)result.Succeeded! ? Ok(result) : BadRequest(result);

        }


        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return NewResult(_responseHandler.Unauthorized<AppUser>());

            var command = new DeleteAppUserCommand(userId);
            var result = await _mediator.Send(command);

            return (bool)result.Succeeded! ? Ok(result) : NotFound(result);

        }

        [HttpDelete("delete-user/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var command = new DeleteAppUserCommand(userId);
            var result = await _mediator.Send(command);
            return (bool)result.Succeeded! ? Ok(result) : NotFound(result);

        }

    }
}
