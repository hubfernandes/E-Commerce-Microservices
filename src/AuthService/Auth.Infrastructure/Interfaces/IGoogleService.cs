using Auth.Domain.Responses;
using Microsoft.AspNetCore.Authentication;

namespace Auth.Infrastructure.Interfaces
{
    public interface IGoogleService
    {
        AuthenticationProperties GetGoogleLoginProperties(string redirectUri);
        Task<AuthResponse> GoogleLoginCallbackAsync();
    }
}
