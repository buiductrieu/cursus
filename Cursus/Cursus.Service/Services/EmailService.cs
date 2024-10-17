using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;

namespace Cursus.Service.Services
{
	public class EmailService : IEmailService
	{
		private readonly IEmailRepository _emailRepository;

		public EmailService(IEmailRepository emailRepository)
		{
			_emailRepository = emailRepository;
		}
		public void SendEmail(EmailRequestDTO request)
		{
			try
			{
				_emailRepository.SendEmail(request);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}

		}

		public void SendEmailConfirmation(string username, string confirmLink)
		{
			EmailRequestDTO request = new EmailRequestDTO
			{
				Subject = "Cursus Email Confirmation",
				Body = "",
				toEmail = username
			};
			_emailRepository.SendEmailConfirmation(request, confirmLink);

		}

		public void SendEmailSuccessfullyPurchasedCourse(ApplicationUser user, Order order)
		{

			EmailRequestDTO request = new EmailRequestDTO
			{
				Subject = "Cursus Email Successfully Purchased Course",
				Body = "",
				toEmail = user.Email
			};

			_emailRepository.SendEmailSuccessfullyPurchasedCourse(request, order);
		}
	}
}
