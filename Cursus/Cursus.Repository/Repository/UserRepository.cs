using Cursus.Data.Entities;
using Cursus.Data.Models;
using Cursus.RepositoryContract.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Repository.Repository
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        private readonly CursusDbContext _db;
        public UserRepository(CursusDbContext db) : base(db)
        {
            _db=db;
        }
        public async Task<ApplicationUser>? ExiProfile(string id)
        {
            return await GetAsync(filter:b => b.Id.Equals(id)) ; 
        }

        public async Task<ApplicationUser> UpdProfile(ApplicationUser usr)
        {
            return await UpdateAsync(usr);
        }
        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _db.Users.AnyAsync(u => u.UserName == username);
        }
    }
}
