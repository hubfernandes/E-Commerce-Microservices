using Auth.Domain.Email;

namespace Auth.Infrastructure.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}
