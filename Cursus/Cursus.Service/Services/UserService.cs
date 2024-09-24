using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public async Task<ApplicationUser> UpdateUserProfile(string id, ApplicationUser usr)
        {
            var O_user = _userRepository.ExiProfile(id);
            if (O_user == null)
            {
                return null;
            }
            await _userRepository.UpdProfile(usr);
            return usr;
        }
    }
}
