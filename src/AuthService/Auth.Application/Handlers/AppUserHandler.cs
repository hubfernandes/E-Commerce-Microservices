using Auth.Application.Commands;
using Auth.Application.Helpers;
using Auth.Domain.Dtos;
using Auth.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.Bases;

namespace Auth.Application.Handlers
{
    public class AppUserHandler : IRequestHandler<RegisterCommand, Response<string>>,
                                   IRequestHandler<UpdateAppUserCommand, Response<AppUserDto>>,
                                   IRequestHandler<UpdateProfileRequest, Response<string>>,
                                   IRequestHandler<DeleteAppUserCommand, Response<string>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        public readonly ResponseHandler _responseHandler;
        private readonly IUserRegistrationHelper _userRegistrationHelper;


        public AppUserHandler(UserManager<AppUser> userManager, IMapper mapper, ResponseHandler responseHandler, IUserRegistrationHelper userRegistrationHelper)
        {
            _userManager = userManager;
            _mapper = mapper;
            _responseHandler = responseHandler;
            _userRegistrationHelper = userRegistrationHelper;
        }

        public async Task<Response<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<AppUser>(request);
            return await _userRegistrationHelper.RegisterUserAsync(user, request.Password!, "admin");
        }

        public async Task<Response<AppUserDto>> Handle(UpdateAppUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id!);
            if (user == null)
            {
                return _responseHandler.NotFound<AppUserDto>("User not found.");
            }

            _mapper.Map(request, user);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return _responseHandler.BadRequest<AppUserDto>(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            var updatedUserDto = _mapper.Map<AppUserDto>(user);
            return _responseHandler.Success(updatedUserDto);
        }

        public async Task<Response<string>> Handle(UpdateProfileRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return _responseHandler.NotFound<string>("User not found.");

            _mapper.Map(request.Command, user);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return _responseHandler.BadRequest<string>(string.Join(", ", result.Errors.Select(e => e.Description)));

            return _responseHandler.Success("Profile updated successfully.");
        }

        public async Task<Response<string>> Handle(DeleteAppUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id!);
            if (user == null)
                return _responseHandler.NotFound<string>("User not found.");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return _responseHandler.BadRequest<string>(string.Join(", ", result.Errors.Select(e => e.Description)));

            return _responseHandler.Success("Account deleted successfully.");
        }
    }
}
