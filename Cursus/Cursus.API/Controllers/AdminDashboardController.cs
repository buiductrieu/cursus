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
    }
}
