using Auth.Domain.Dtos;
using MediatR;
using Shared.Wrapper;

namespace Auth.Application.Queries
{
    public record GetAllAppUsers(int PageNumber, int PageSize) : IRequest<PaginatedResult<AppUserDto>>;

}
