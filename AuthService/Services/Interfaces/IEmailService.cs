using AuthService.DTOs.EmailDtos;

namespace AuthService.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailRequest request);
    }
}
