using Cursus.Data.DTO;

namespace Cursus.ServiceContract.Interfaces
{
    public interface IEmailService
    {
        public void SendEmail(EmailRequestDTO request);
        public void SendEmailConfirmation(string username, string confirmLink);
    }
}
