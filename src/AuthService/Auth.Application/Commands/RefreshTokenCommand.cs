using Auth.Domain.Responses;
using MediatR;
using Shared.Bases;

namespace Auth.Application.Commands
{
    public record RefreshTokenCommand(string? AccessToken, string? RefreshToken)
        : IRequest<Response<AuthResponse>>;
}