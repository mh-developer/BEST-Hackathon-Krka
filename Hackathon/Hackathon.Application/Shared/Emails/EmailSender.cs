using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Hackathon.Application.Shared.Emails
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            Execute(email, subject, message).Wait();
            return Task.CompletedTask;
        }

        private async Task Execute(string email, string subjectMessage, string message)
        {
            var apiKey = _configuration["SendGridAPI"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("noreply@hekatlon.com", "Hekatlon");
            var subject = subjectMessage;
            var to = new EmailAddress(email);
            var plainTextContent = message;
            var htmlContent = message + " <strong>Za namene hekatlona.</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}