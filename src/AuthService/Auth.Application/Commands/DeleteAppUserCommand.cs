using MediatR;
using Shared.Bases;

namespace Auth.Application.Commands
{
    public record DeleteAppUserCommand(string? Id)
        : IRequest<Response<string>>;
}