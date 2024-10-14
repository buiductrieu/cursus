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
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<bool> AdminComments(string userId, string comment)
        {
            return await _adminRepository.AdminComments(userId, comment);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUser()
        {
            return  await _adminRepository.GetAllAsync();
        }

        public async Task<bool> ToggleUserStatusAsync(string userId)
        {
            return await _adminRepository.ToggleUserStatusAsync(userId);
        }


    }
}
