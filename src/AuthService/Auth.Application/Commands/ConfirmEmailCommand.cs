using Auth.Domain.Entities;
using MediatR;
using Shared.Bases;

namespace Auth.Application.Commands
{
    public record ConfirmEmailCommand(string UserId, string Token)
        : IRequest<Response<AppUser>>;
}