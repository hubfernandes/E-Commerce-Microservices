using Auth.Domain.Responses;
using MediatR;

namespace Auth.Application.Commands
{
    public record GoogleLoginCallbackCommand()
        : IRequest<AuthResponse>;
}