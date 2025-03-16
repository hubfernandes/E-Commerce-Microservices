using MediatR;
using Shared.Bases;

namespace Auth.Application.Commands
{
    public record ChangePasswordCommand(string? CurrentPassword, string? NewPassword, string? ConfirmNewPassword)
        : IRequest<Response<string>>;
}