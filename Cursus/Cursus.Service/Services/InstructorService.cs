using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.Data.Enum;
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

        public InstructorService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IEmailService emailService )
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }
        public async Task<ApplicationUser> InstructorAsync(RegisterInstructorDTO registerInstructorDTO)
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

            if (userResult.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "Instructor");

                if (!roleResult.Succeeded)
                {
                    return null; // Gán vai trò không thành công
                }

                var instructorInfo = new InstructorInfo
                {
                    UserId = user.Id,
                    CardName = registerInstructorDTO.CardName,
                    CardProvider = registerInstructorDTO.CardProvider,
                    CardNumber = registerInstructorDTO.CardNumber,
                    SubmitCertificate = registerInstructorDTO.SubmitCertificate,
                    StatusInsructor = InstructorStatus.Pending
                };

                await _unitOfWork.InstructorInfoRepository.AddAsync(instructorInfo);
                await _unitOfWork.SaveChanges();

                return user; // Trả về đối tượng ApplicationUser để tiếp tục xử lý
            }

            return null; // Đăng ký không thành công
        }

        public async Task<IdentityResult> ConfirmInstructorEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
            }
            return result;
        }

        public async Task<bool> ApproveInstructorAsync(string instructorId)
        {
            var instructorInfo = await _unitOfWork.InstructorInfoRepository.GetAsync(x => x.UserId == instructorId);
            if (instructorInfo == null) throw new KeyNotFoundException("Instuctor not found");

            instructorInfo.StatusInsructor = InstructorStatus.Approved;
            await _unitOfWork.InstructorInfoRepository.UpdateAsync(instructorInfo);
            await _unitOfWork.SaveChanges();

            var user = await _userManager.FindByIdAsync(instructorInfo.UserId);
            var emailRequest = new EmailRequestDTO
            {
                toEmail = user.Email,
                Subject = "Instructor Account Approved",
                Body = $"Dear {user.UserName},<br>Your instructor account has been approved and activated. You can now access the system."
            };
            _emailService.SendEmail(emailRequest);

            return true;
        }


        public async Task<bool> RejectInstructorAsync(string instructorId)
        {
            var instructorInfo = await _unitOfWork.InstructorInfoRepository.GetAsync(x => x.UserId == instructorId);
            if (instructorInfo == null) throw new KeyNotFoundException("Instuctor not found");

            instructorInfo.StatusInsructor = InstructorStatus.Rejected;
            await _unitOfWork.InstructorInfoRepository.UpdateAsync(instructorInfo);
            await _unitOfWork.SaveChanges();

            var user = await _userManager.FindByIdAsync(instructorInfo.UserId);
            var emailRequest = new EmailRequestDTO
            {
                toEmail = user.Email,
                Subject = "Instructor Account Rejected",
                Body = $"Dear {user.UserName},<br>Your instructor account registration has been rejected. Please contact support for further information."
            };
            _emailService.SendEmail(emailRequest);

            return true;
        }
    }
}
