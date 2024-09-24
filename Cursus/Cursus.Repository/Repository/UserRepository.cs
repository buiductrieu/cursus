using Cursus.Data.Entities;
using Cursus.Data.Models;
using Cursus.RepositoryContract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Repository.Repository
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    { 
        public UserRepository(CursusDbContext db) : base(db)
        {
        }
        public async Task<bool> ExiProfile(string id)
        {
            return await GetAsync(filter:b => b.Id == id) !=null; 
        }

        public async Task<ApplicationUser> UpdProfile(ApplicationUser usr)
        {
            return await UpdateAsync(usr);
        }
    }
}
