using MediatR;
using Shared.Bases;

namespace Auth.Application.Commands
{
    public record ForgotPasswordCommand(string? Email)
        : IRequest<Response<string>>;
}