using Auth.Domain.Entities;
using Shared.Bases;

namespace Auth.Application.Helpers
{
    public interface IUserRegistrationHelper
    {
        Task<Response<string>> RegisterUserAsync(AppUser user, string password, string role);
    }
}
