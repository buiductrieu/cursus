using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.Data.Models;
using Cursus.RepositoryContract.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cursus.Repository.Repository
{
    public class BookmarkRepository : IBookmarkRepository
    {
        private readonly CursusDbContext _db;

        public BookmarkRepository(CursusDbContext db)
        {
            _db = db;
        }

        // Method to get bookmarks with filtering and sorting functionality
        public async Task<IEnumerable<BookmarkDTO>> GetFilteredAndSortedBookmarksAsync(string userId, string? courseName, int? courseId, string? sortBy, string sortOrder)
        {
            var query = _db.Bookmarks
                .Include(b => b.Course)
                .Where(b => b.UserId == userId)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(courseName))
            {
                query = query.Where(b => b.Course.Name.Contains(courseName));
            }

            if (courseId.HasValue)
            {
                query = query.Where(b => b.CourseId == courseId.Value);
            }

            // Apply sorting
            switch (sortBy?.ToLower())
            {
                case "coursename":
                    query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(b => b.Course.Name) : query.OrderBy(b => b.Course.Name);
                    break;
                case "price":
                    query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(b => b.Course.Price) : query.OrderBy(b => b.Course.Price);
                    break;
                case "rating":
                    query = sortOrder.ToLower() == "desc" ? query.OrderByDescending(b => b.Course.Rating) : query.OrderBy(b => b.Course.Rating);
                    break;
                default:
                    query = query.OrderBy(b => b.Id);
                    break;
            }

            return await query.Select(b => new BookmarkDTO
            {
                Id = b.Id,
                CourseName = b.Course.Name,
                Summary = b.Course.Description,
                Price = b.Course.Price,
                Rating = b.Course.Rating
            }).ToListAsync();
        }

        // Method to get course details by courseId
        public async Task<CourseDetailDTO> GetCourseDetailsAsync(int courseId)
        {
            var course = await _db.Courses
                .Include(c => c.Steps)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null) return null;

            return new CourseDetailDTO
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                Instructor = "Instructor Name", // Placeholder, you may replace with actual data
                Steps = course.Steps.Select(step => new StepDTO
                {
                    Id = step.Id,
                    CourseId = step.CourseId,
                    Name = step.Name,
                    Description = step.Description
                }).ToList()
            };
        }

        // Method to add a new bookmark
        public async Task AddAsync(Bookmark bookmark)
        {
            await _db.Bookmarks.AddAsync(bookmark);
          //  await _db.SaveChangesAsync();
        }
    }
}
