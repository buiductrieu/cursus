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
    public class UserRepository : Repository<ApplicationUser>, IUserRepositiory
    { 
        public UserRepository(CursusDbContext db) : base(db, db.Set<ApplicationUser>())
        {
        }
        public async Task<ApplicationUser> ExiProfile(string id)
        {
            return await GetAsync(filter:b => b.Id == id); 
        }

        public Task<bool> UpdProfile(ApplicationUser usr)
        {
            throw new NotImplementedException();
        }
    }
}
