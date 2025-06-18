namespace OVOVAX.Core.Services.Contract
{
    public interface IEmailService
    {
        Task<bool> SendPasswordResetEmailAsync(string email, string resetLink);
        Task<bool> SendWelcomeEmailAsync(string email, string firstName);
        Task<bool> SendEmailAsync(string to, string subject, string htmlBody);
    }
}
