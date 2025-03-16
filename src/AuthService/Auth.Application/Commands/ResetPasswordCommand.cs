using MediatR;
using Shared.Bases;

namespace Auth.Application.Commands
{
    public record ResetPasswordCommand(string? Email, string? Token, string? NewPassword, string? ConfirmNewPassword)
        : IRequest<Response<string>>;
}