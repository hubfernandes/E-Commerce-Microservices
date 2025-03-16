using Auth.Domain.Entities;
using Auth.Domain.Responses;

namespace Auth.Infrastructure.Interfaces
{
    public interface IJwtService
    {
        Task<AuthResponse> GenerateJwtToken(AppUser user);
        Task<AuthResponse> RefreshToken(string expiredToken, string refreshToken);
    }
}
