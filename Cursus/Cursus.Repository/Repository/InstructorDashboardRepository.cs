using Cursus.Data.DTO;
using Cursus.Data.Models;
using Cursus.RepositoryContract.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Repository.Repository
{
    public class InstructorDashboardRepository : IInstructorDashboardRepository
    {
        private readonly CursusDbContext _context; // Replace with your actual DbContext

        public InstructorDashboardRepository(CursusDbContext context)
        {
            _context = context;
        }

        // Method to get the instructor dashboard details
        public async Task<InstructorDashboardDTO> GetInstructorDashboardAsync(int instructorId)
        {
            var courses = await _context.Courses
                .Include(course => course.InstructorInfo)
                .Where(course => course.InstructorInfo.Id == instructorId && course.Status)
                .ToListAsync();

            var totalPotentialEarnings = courses.Sum(course => course.Price);
            var totalEarnings = courses.Sum(course => course.InstructorInfo.TotalEarning); 

            return new InstructorDashboardDTO
            {
                TotalPotentialEarnings = totalPotentialEarnings,
                TotalCourses = courses.Count,
                TotalEarnings = totalEarnings
            };
        }
        public async Task<List<CourseEarningsDTO>> GetCourseEarningsAsync(int instructorId)
        {
            var courses = await _context.Courses
                .Include(course => course.InstructorInfo)
                .Where(course => course.InstructorInfo.Id == instructorId && course.Status)
                .ToListAsync();

            return courses.Select(course => new CourseEarningsDTO
            {
                Status = course.Status ? "Active" : "Deactive",
                ShortSummary = course.Description.Length > 100 ? course.Description.Substring(0, 100) : course.Description,
 //             Earnings = course.Earnings,
 //             PotentialEarnings = course.PotentialEarnings,
                Price = course.Price
            }).ToList();
        }
    }
}
