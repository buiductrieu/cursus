using Cursus.Data.Entities;
using Cursus.Repository.Repository;
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
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IInstructorInfoRepository _instructorInfoRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminService(IAdminRepository adminRepository, IInstructorInfoRepository instructorInfoRepository, UserManager<ApplicationUser> userManager)
        {
            _adminRepository = adminRepository;
            _instructorInfoRepository = instructorInfoRepository;
            _userManager = userManager;
        }

        public async Task<bool> AdminComments(string userId, string comment)
        {
            return await _adminRepository.AdminComments(userId, comment);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUser()
        {
            return  await _adminRepository.GetAllAsync();
        }

        public async Task<Dictionary<string, object>?> GetInformationInstructor(int instructorId)
        {
            var instructor = await _adminRepository.GetInformationInstructorAsync(instructorId);
            var details = new Dictionary<string, object>();
            var allInstructors = await _instructorInfoRepository.GettAllAsync();

            // Kiểm tra thông tin instructor
            if (string.IsNullOrEmpty(instructor.UserName))
            {
                details.Add("Error", "Instructor not found");
            }
            else
            {
                // Thêm thông tin người dùng vào chi tiết
                details.Add("UserName", instructor.UserName ?? string.Empty);
                details.Add("Email", instructor.Email ?? string.Empty);
                details.Add("PhoneNumber", instructor.PhoneNumber ?? string.Empty);
                details.Add("AdminComment", instructor.AdminComment ?? string.Empty);
            }
            var totalEarning = allInstructors
                     .Where(i => i.Id == instructorId)
                     .Sum(i => i.TotalEarning);
            details.Add("TotalEarning", totalEarning);
            var totalCourses = await _instructorInfoRepository.TotalCourse(instructorId); // Lấy tổng số khóa học
            details.Add("TotalCourses", totalCourses);

            // Lấy tổng số khóa học đang hoạt động
            var totalActiveCourses = await _instructorInfoRepository.TotalActiveCourse(instructorId);
            details.Add("TotalActiveCourses", totalActiveCourses);

            // Tính tổng thu nhập
            var totalPayout = await _instructorInfoRepository.TotalPayout(instructorId);
            details.Add("TotalPayout", totalPayout);

            // Tính trung bình đánh giá
            var averageRating = await _instructorInfoRepository.RatingNumber(instructorId);
            details.Add("AverageRating", averageRating);

            return details;
        }





        public async Task<bool> ToggleUserStatusAsync(string userId)
        {
            return await _adminRepository.ToggleUserStatusAsync(userId);
        }

   
    }
}
