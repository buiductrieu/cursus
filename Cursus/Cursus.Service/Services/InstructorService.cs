using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Service.Services
{
    public class InstructorService : IInstructorService
    {
        public readonly UserManager<ApplicationUser> _userManager;
        public readonly IUnitOfWork _unitOfWork;

        public InstructorService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;   
        }
        public async Task<IdentityResult> InstructorAsync(RegisterInstructorDTO registerInstructorDTO)
        {
            var user = new ApplicationUser
            {
                UserName = registerInstructorDTO.Email,
                Email = registerInstructorDTO.Email,
                PhoneNumber = registerInstructorDTO.Phone,
                Address = registerInstructorDTO.Address
            };

            var userResult = await _userManager.CreateAsync(user, registerInstructorDTO.Password);

            if(userResult.Succeeded)
            {

                // Thêm vai trò "Instructor" cho người dùng
                var roleResult = await _userManager.AddToRoleAsync(user, "Instructor");

                // Kiểm tra xem việc gán role có thành công không
                if (!roleResult.Succeeded)
                {
                    return IdentityResult.Failed(roleResult.Errors.ToArray());
                }

                var userId = user.Id;
                var instructorInfo = new InstructorInfo
                {

                    UserId = userId,
                    CardName = registerInstructorDTO.CardName,
                    CardProvider = registerInstructorDTO.CardProvider,
                    CardNumber = registerInstructorDTO.CardNumber,
                    SubmitCertificate = registerInstructorDTO.SubmitCertificate
                };

                await _unitOfWork.InstructorInfoRepository.AddAsync(instructorInfo);
                await _unitOfWork.SaveChanges();
            }
            return userResult;
        }
    }
}
