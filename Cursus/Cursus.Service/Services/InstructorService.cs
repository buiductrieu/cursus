using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.Data.Enum;
using Cursus.Data.Models;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private readonly CursusDbContext _context;


        public InstructorService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IEmailService emailService, CursusDbContext context)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _context = context;

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
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = $"https://yourapplication.com/confirm?userId={user.Id}&email={user.Email}";
                try
                {              
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
            var instructorInfo = await _unitOfWork.InstructorInfoRepository.GetByIDAsync(int.Parse(instructorId));
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
            var instructorInfo = await _unitOfWork.InstructorInfoRepository.GetByIDAsync(int.Parse(instructorId));
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

        public async Task<IEnumerable<InstructorInfo>> GetAllInstructors()
        {
            var instructors = await _context.InstructorInfos
                                                       .Include(i => i.User) 
                                                       .ToListAsync();


            return instructors;
        }
    }
}
