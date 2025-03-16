using MediatR;
using Microsoft.AspNetCore.Authentication;

namespace Auth.Application.Commands
{
    public record GoogleLoginCommand(string? RedirectUri)
        : IRequest<AuthenticationProperties>;
}