using Cursus.Data.Entities;

namespace Cursus.RepositoryContract.Interfaces
{
    public interface IUserRepositiory
    {
        public Task<bool> UpdProfile(ApplicationUser usr);
        public Task<ApplicationUser> ExiProfile(string id);
    }
}
