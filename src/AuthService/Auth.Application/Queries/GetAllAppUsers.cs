using Auth.Domain.Dtos;
using MediatR;
using Shared.Wrapper;

namespace Auth.Application.Queries
{
    public class GetAllAppUsers : IRequest<PaginatedResult<AppUserDto>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

}
