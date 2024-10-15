using Cursus.Common.Helper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
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

        public void SendEmailConfirmation(EmailRequestDTO request, string confirmLink)
        {
            var body = $"<h1>Email Confirmation</h1><p>Dear {request.toEmail},</p><p>Thank you for registering with us. Please confirm your email by clicking on the link below.</p><a href='{confirmLink}'>Click here to confirm your email</a>";
            request.Body = body;
            SendEmail(request);
        }

		public void SendEmailSuccessfullyPurchasedCourse(EmailRequestDTO request, Order order)
		{
			var body = $@"
<h1>Course Purchase Confirmation</h1>
<p>Dear {request.toEmail},</p>
<p>We are excited to inform you that your purchase of the following courses has been successfully completed!</p>

<h3>Purchase Details:</h3>
<ul>";
			foreach (var item in order.Cart.CartItems)
			{
				body += $@"
        <li>
            <strong>Course Name:</strong> {item.Course.Name}<br />
            <strong>Price:</strong> {item.Course.Price}<br />
        </li>";
			}
			body += $@"
</ul>
<p><strong>Total Paid:</strong> {order.PaidAmount}</p>
<p><strong>Order ID:</strong> {order.OrderId}</p>

<p>If you have any questions, feel free to contact our support team at <a href='mailto:{_emailSetting.Email}'></a>.</p>

<p>Thank you for choosing us! We wish you the best of luck in your learning journey.</p>

<p>Warm regards,</p>
<p><strong>Cursus</strong></p>";
			request.Body = body;
			SendEmail(request);
		}
	}
}
