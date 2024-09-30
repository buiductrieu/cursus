using Cursus.Data.Entities;

namespace Cursus.RepositoryContract.Interfaces
{
    public interface IUserRepository
    {
        public Task<ApplicationUser> UpdProfile(ApplicationUser usr);
        public Task<ApplicationUser>? ExiProfile(string id);
        public Task<bool> UsernameExistsAsync(string username);
        public Task<bool> PhoneNumberExistsAsync(string phoneNumber);
    }
}
