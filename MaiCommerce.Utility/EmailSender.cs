using Microsoft.AspNetCore.Identity.UI.Services;

namespace MaiCommerce.Utility;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        //TODO: Implement actual email sending service
        return Task.CompletedTask;
    }
}