using Cursus.ServiceContract.Interfaces;
using Cursus.Data.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cursus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _adminDashboardService;

        public AdminDashboardController(IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService = adminDashboardService;
        }

        [HttpGet("top-purchased-courses")]
        public async Task<ActionResult<List<PurchaseCourseOverviewDTO>>> GetTopPurchasedCourses(int year, string period)
        {
            var courses = await _adminDashboardService.GetTopPurchasedCourses(year, period);

            if (courses == null || courses.Count == 0)
            {
                return NotFound("No courses found for the specified year and period.");
            }

            return Ok(courses);
        }

        [HttpGet("worst-rated-courses")]
        public async Task<ActionResult<List<PurchaseCourseOverviewDTO>>> GetWorstRatedCourses(int year, string period)
        {
            var courses = await _adminDashboardService.GetWorstRatedCourses(year, period);

            if (courses == null || courses.Count == 0)
            {
                return NotFound("No worst rated courses found for the specified year and period.");
            }

            return Ok(courses);
        }

        [HttpGet("total-users")]
        public async Task<IActionResult> GetTotalUsers()
        {
            var totalUsers = await _adminDashboardService.GetTotalUsersAsync();
            return Ok(totalUsers);
        }

        [HttpGet("total-instructors")]
        public async Task<IActionResult> GetTotalInstructors()
        {
            var totalInstructors = await _adminDashboardService.GetTotalInstructorsAsync();
            return Ok(totalInstructors);
        }
    }
}
