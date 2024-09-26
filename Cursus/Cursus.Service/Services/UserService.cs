using Cursus.Data.Entities;
using Cursus.Repository.Repository;
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
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<ApplicationUser> UpdateUserProfile(string id, ApplicationUser usr)
        {
            return await _userRepository.UpdProfile(usr);
        }
    }
}
