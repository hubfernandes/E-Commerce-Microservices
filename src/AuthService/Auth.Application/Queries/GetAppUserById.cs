using Auth.Domain.Dtos;
using MediatR;
using Shared.Bases;

namespace Auth.Application.Queries
{
    public record GetAppUserById(string UserId) : IRequest<Response<AppUserDto>>;
}
