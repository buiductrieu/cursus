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


        public void SendCertificateEmail(EmailRequestDTO request, string certificateDownloadUrl)
        {
            var body =  $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Certificate Completion</title>
    <style>
        body {{
            font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            background-color: #f9fafb;
            margin: 0;
            padding: 20px;
            color: #1f2937;
        }}
        .email-container {{
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            border-radius: 16px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
            overflow: hidden;
        }}
        .email-header {{
            background-color: #4f46e5;
            padding: 32px 24px;
            text-align: center;
        }}
        .email-header h1 {{
            margin: 0;
            color: #ffffff;
            font-size: 24px;
            font-weight: 700;
        }}
        .email-body {{
            padding: 32px 24px;
            font-size: 16px;
            line-height: 1.6;
        }}
        .email-footer {{
            text-align: center;
            padding: 24px;
            background-color: #f9fafb;
            font-size: 14px;
            color: #6b7280;
        }}
        .email-footer a {{
            color: #4f46e5;
            text-decoration: none;
        }}
    </style>
</head>
<body>
    <div class=""email-container"">
        <div class=""email-header"">
            <h1>Congratulations on Completing Your Course!</h1>
        </div>
        <div class=""email-body"">
            <p>Dear {request.toEmail},</p>
            <p>We are excited to inform you that you have successfully completed your course and earned a certificate of completion!</p>
            
            <h3>Certificate Download:</h3>
            <p>You can download your certificate by clicking the link below:</p>
           <p style=""text-align: center;"">
    <a href='{certificateDownloadUrl}' target='_blank' 
       style='display: inline-block;
              padding: 12px 24px;
              color: #ffffff;
              background-color: #4f46e5;
              border-radius: 8px;
              text-decoration: none;
              font-weight: bold;
              box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);'>
       Download Your Certificate
    </a>
</p>

            <p>Thank you for learning with us! We wish you all the best in your future studies.</p>

            <p>Warm regards,</p>
            <p><strong>Cursus</strong></p>
        </div>
        <div class=""email-footer"">
            <p>If you have any questions, feel free to reach out to <a href=""mailto:support@cursus.com"">support@cursus.com</a></p>
            <p>&copy; 2024 Cursus. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";


            request.Body = body;
            SendEmail(request); 
        }




    }
}
