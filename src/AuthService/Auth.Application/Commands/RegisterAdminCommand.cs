using MediatR;
using Shared.Bases;

namespace Auth.Application.Commands
{
    public record RegisterAdminCommand(string? UserName, string? Password, string? ConfirmPassword, string? Email)
        : IRequest<Response<string>>;

    public record RegisterUserCommand : RegisterAdminCommand, IRequest<Response<string>>
    {
        public RegisterUserCommand(string? UserName, string? Password, string? ConfirmPassword, string? Email) : base(UserName, Password, ConfirmPassword, Email)
        {
        }
    }
}