using Cursus.Data.Entities;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Service.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

      
        public async Task<bool> CheckUserExistsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user != null;
        }
    }
}
