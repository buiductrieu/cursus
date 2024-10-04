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
        private readonly IEmailService _emailService;

        public InstructorService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }
        public async Task<IdentityResult> InstructorAsync(RegisterInstructorDTO registerInstructorDTO)
        {
            var user = new ApplicationUser
            {
                UserName = registerInstructorDTO.Email,
                Email = registerInstructorDTO.Email,
                PhoneNumber = registerInstructorDTO.Phone,
                Address = registerInstructorDTO.Address,
                EmailConfirmed = false
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
                    SubmitCertificate = registerInstructorDTO.SubmitCertificate,
                };

                await _unitOfWork.InstructorInfoRepository.AddAsync(instructorInfo);
                await _unitOfWork.SaveChanges();
                try
                {
                    var confirmationLink = $"https://yourapplication.com/confirm?userId={user.Id}&email={user.Email}";
                    _emailService.SendEmailConfirmation(user.Email, confirmationLink);
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu việc gửi email thất bại
                    return IdentityResult.Failed(new IdentityError { Description = $"User registered successfully, but failed to send confirmation email. Error: {ex.Message}" });
                }
            }
            return userResult;
        }
    }
}
