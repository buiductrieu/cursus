using Cursus.Data.DTO;
using Cursus.Data.Models;
using Cursus.RepositoryContract.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursus.Repository.Repository
{
    public class AdminDashboardRepository : IAdminDashboardRepository
    {
        private readonly CursusDbContext _context;

        public AdminDashboardRepository(CursusDbContext context)
        {
            _context = context;
        }

        public async Task<List<PurchaseCourseOverviewDTO>> GetTopPurchasedCourses(int year, string period)
        {
            var cartsQuery = _context.Cart
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Course) 
                .Where(c => c.IsPurchased) 
                .Where(c => c.CartItems.Any()) 
                .AsQueryable();

          
            if (period.ToLower() == "month")
            {
                cartsQuery = cartsQuery.Where(c => c.CartItems
                    .Any(ci => ci.Course.DateCreated.Year == year &&
                               ci.Course.DateCreated.Month >= 1 && ci.Course.DateCreated.Month <= 12));
            }
            else if (period.ToLower() == "quarter")
            {
                cartsQuery = cartsQuery.Where(c => c.CartItems
                    .Any(ci => ci.Course.DateCreated.Year == year &&
                               ci.Course.DateCreated.Month >= 1 && ci.Course.DateCreated.Month <= 3));
            }

            var topCourses = await cartsQuery
                .SelectMany(c => c.CartItems)
                .GroupBy(ci => ci.Course.Id) 
                .OrderByDescending(g => g.Count())
                .Take(5) 
                .Select(g => new PurchaseCourseOverviewDTO
                {
                    Id = g.Key, 
                    CourseName = g.FirstOrDefault().Course.Name,
                    Summary = g.FirstOrDefault().Course.Description,
                    Price = g.FirstOrDefault().Course.Price,
                    StepCount = g.FirstOrDefault().Course.Steps.Count
                })
                .ToListAsync();

            return topCourses;
        }
    }
}
