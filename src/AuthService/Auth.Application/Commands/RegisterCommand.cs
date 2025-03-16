using MediatR;
using Shared.Bases;

namespace Auth.Application.Commands
{
    public record RegisterCommand(string? UserName, string? Password, string? ConfirmPassword, string? Email)
        : IRequest<Response<string>>;
}