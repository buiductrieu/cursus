using Cursus.Common.Helper;
using Cursus.Data.DTO;
using Cursus.RepositoryContract.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Cursus.Repository.Repository
{
    public class EmailRepository : IEmailRepository
    {

        private readonly EmailSetting _emailSetting;
        private readonly ILogger<EmailRepository> _logger;
        public EmailRepository(IOptions<EmailSetting> options, ILogger<EmailRepository> logger)
        {
            _emailSetting = options.Value;
            _logger = logger;
        }


        public void SendEmail(EmailRequestDTO request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_emailSetting.Email));
            email.To.Add(MailboxAddress.Parse(request.toEmail));
            email.Subject = request.Subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = request.Body;
            email.Body = builder.ToMessageBody();
            try
            {
                using var smtp = new SmtpClient();

                smtp.Connect(_emailSetting.Host, _emailSetting.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_emailSetting.Email, _emailSetting.Password);
                smtp.Send(email);
                smtp.Disconnect(true);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to send email to {Email}", request.toEmail);
                throw new Exception(e.Message);
            }

        }
    }
}
