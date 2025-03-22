using Auth.Application.Queries;
using Auth.Domain.Dtos;
using Auth.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.Bases;
using Shared.Wrapper;

namespace Auth.Application.Handlers
{
    internal class AppUserQueryHandler : IRequestHandler<GetAppUserById, Response<AppUserDto>>,
                                       IRequestHandler<GetAllAppUsers, PaginatedResult<AppUserDto>>


    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        public readonly ResponseHandler _responseHandler;

        public AppUserQueryHandler(UserManager<AppUser> userManager, IMapper mapper, ResponseHandler responseHandler)
        {
            _userManager = userManager;
            _mapper = mapper;
            _responseHandler = responseHandler;

        }
        public async Task<Response<AppUserDto>> Handle(GetAppUserById request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return _responseHandler.Unauthorized<AppUserDto>("Unauthorized User");
            }

            var userDto = _mapper.Map<AppUserDto>(user);
            return _responseHandler.Success(userDto);
        }

        public async Task<PaginatedResult<AppUserDto>> Handle(GetAllAppUsers request, CancellationToken cancellationToken)
        {
            var allUsers = _userManager.Users.AsQueryable();
            var paginatedResult = _mapper.ProjectTo<AppUserDto>(allUsers).ToPaginatedListAsync(request.PageNumber, request.PageSize);

            return await paginatedResult;
        }
    }
}
