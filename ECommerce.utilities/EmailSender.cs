using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.utilities
{
    public class EmailSender : IEmailSender
    {
        public string SendGridKey { get; set; }

        public EmailSender(IConfiguration _config)
        {
            SendGridKey = _config.GetValue<string>("SendGrid:SecretKey");
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SendGridClient(SendGridKey);
            var from = new EmailAddress("OnlineBookStore@ECommerce.com", "Online Book Store");

            var to = new EmailAddress(email);

            var message = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);
            
            return client.SendEmailAsync(message);
        }
    }
}
