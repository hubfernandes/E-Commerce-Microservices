using Auth.Domain.Dtos;
using MediatR;
using Shared.Bases;

namespace Auth.Application.Commands
{
    public record UpdateAppUserCommand(string? Id, string? UserName, string? Email, string? PhoneNumber)
        : IRequest<Response<AppUserDto>>; // For admin

    public record UpdateProfileCommand(string? UserName, string? Email, string? PhoneNumber)
        : IRequest<Response<string>>; // For user

    public record UpdateProfileRequest(string UserId, UpdateProfileCommand Command)
        : IRequest<Response<string>>;
}