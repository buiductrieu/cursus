using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Cursus.Service.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IInstructorInfoRepository _instructorInfoRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(IAdminRepository adminRepository, IInstructorInfoRepository instructorInfoRepository, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _adminRepository = adminRepository;
            _instructorInfoRepository = instructorInfoRepository;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AcceptPayout(int transactionId)
        {
            var transaction = await _unitOfWork.TransactionRepository.GetAsync(t => t.TransactionId == transactionId);

            if (transaction == null)
            {
                throw new KeyNotFoundException("Transaction not found");
            }

            if (!transaction.Description.Contains("payout"))
            {
                throw new BadHttpRequestException("Can not confirm this transaction!");
            }

            transaction.Status = Data.Enums.TransactionStatus.Completed;

            var instructorWallet = await _unitOfWork.WalletRepository.GetAsync(w => w.UserId == transaction.UserId);

            instructorWallet.Balance -= transaction.Amount;

            await _unitOfWork.SaveChanges();

            return true;
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
            var userId = _unitOfWork.InstructorInfoRepository.GetAsync(i => i.Id == instructorId).Result.UserId; // Lấy id người dùng
            var instructorWallet = await _unitOfWork.WalletRepository.GetAsync(w => w.UserId == userId);
            var instructor = await _adminRepository.GetInformationInstructorAsync(instructorId);
            var details = new Dictionary<string, object>();

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
            var totalEarning = instructorWallet?.Balance ?? 0; // Nếu không có ví, đặt mặc định là 0
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
