using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cursus.Data.Entities;
using Cursus.Data.Models;
using Cursus.RepositoryContract.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
namespace Cursus.Repository.Repository
{
    public class AdminRepository : Repository<ApplicationUser>, IAdminRepository
    {
        private readonly UserManager<ApplicationUser> userManager;

       
        public AdminRepository(CursusDbContext db, UserManager<ApplicationUser> userManager) : base(db)
        {
            this.userManager = userManager;
        }


        public async Task<bool> ToggleUserStatusAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.Status = !user.Status;

            var result = await userManager.UpdateAsync(user);
            return result.Succeeded;
        }

    


    }
}
