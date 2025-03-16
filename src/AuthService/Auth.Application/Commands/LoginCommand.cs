using Auth.Domain.Responses;
using MediatR;
using Shared.Bases;

namespace Auth.Application.Commands
{
    public record LoginCommand(string? Email, string? Password)
        : IRequest<Response<AuthResponse>>;
}