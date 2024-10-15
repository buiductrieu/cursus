using AutoMapper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.Data.Enum;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Service.Services
{
    public class InstructorService : IInstructorService
    {
        public readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public InstructorService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IEmailService emailService , IMapper mapper)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _mapper = mapper;
        }
        public async Task<ApplicationUser> InstructorAsync(RegisterInstructorDTO registerInstructorDTO)
        {
            var context = new ValidationContext(registerInstructorDTO);
            var user = new ApplicationUser
            {
                UserName = registerInstructorDTO.UserName,
                Email = registerInstructorDTO.UserName,
                PhoneNumber = registerInstructorDTO.Phone,
                Address = registerInstructorDTO.Address,
                EmailConfirmed = false
            };

            var userResult = await _userManager.CreateAsync(user, registerInstructorDTO.Password);

            if (userResult.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "Instructor");

                var instructorInfo = new InstructorInfo
                {
                    UserId = user.Id,
                    CardName = registerInstructorDTO.CardName,
                    CardProvider = registerInstructorDTO.CardProvider,
                    CardNumber = registerInstructorDTO.CardNumber,
                    SubmitCertificate = registerInstructorDTO.SubmitCertificate,
                    TotalEarning = registerInstructorDTO.TotalEarning,
                    StatusInsructor = InstructorStatus.Pending
                };

                await _unitOfWork.InstructorInfoRepository.AddAsync(instructorInfo);
                await _unitOfWork.SaveChanges();

                return user;
            }

            return null;
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

        public async Task<List<InstuctorTotalEarnCourseDTO>> GetTotalAmountAsync(int instructorId)
        {
            var instructorInfo = await _unitOfWork.InstructorInfoRepository.GetAsync(x => x.Id == instructorId);
            if (instructorInfo == null)
                throw new KeyNotFoundException("Instructor is not found");
            var course = await _unitOfWork.CourseRepository.GetAllAsync(c => c.InstructorInfoId == instructorId);
            if (!course.Any() || course == null)
                throw new KeyNotFoundException("\"No courses found for this instructor.");
            var courseSummaryDTOs = _mapper.Map<List<InstuctorTotalEarnCourseDTO>>(course);
            foreach (var item in courseSummaryDTOs)
            {
                item.Earnings = item.Price;
                item.InstructorName = instructorInfo.User?.UserName;
                item.Id = instructorInfo.Id;
            }

            return courseSummaryDTOs;


        }
    }
}
