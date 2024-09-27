using Cursus.Common.Helper;
using Cursus.Data.DTO;

namespace Cursus.RepositoryContract.Interfaces
{
    public interface IEmailRepository
    {
        public void SendEmail(EmailRequestDTO request);
        public void SendEmailConfirmation(EmailRequestDTO request, string confirmLink);
    }
}
