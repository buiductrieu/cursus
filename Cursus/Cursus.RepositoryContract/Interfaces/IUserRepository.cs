using Cursus.Data.Entities;

namespace Cursus.RepositoryContract.Interfaces
{
    public interface IUserRepository
    {
        public Task<ApplicationUser> UpdProfile(ApplicationUser usr);
        public Task<ApplicationUser>? ExiProfile(string id);
    }
}
