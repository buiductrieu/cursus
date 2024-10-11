using Cursus.Data.Entities;
using Cursus.Data.DTO;
using Cursus.RepositoryContract.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cursus.Data.DTO.Cursus.Data.DTO;
using Cursus.Data.Models;

namespace Cursus.Repository.Repository
{
    public class BookmarkRepository : Repository<Bookmark>, IBookmarkRepository
    {
        private readonly CursusDbContext _db;

        public BookmarkRepository(CursusDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<BookmarkDTO>> GetBookmarksByUserIdAsync(string userId)
        {
            return await _db.Bookmarks
                .Include(b => b.Course)
                .Where(b => b.UserId == userId)
                .Select(b => new BookmarkDTO
                {
                    Id = b.Id,
                    CourseName = b.Course.Name,
                    Summary = b.Course.Description,
                    Price = b.Course.Price,
                    Rating = b.Course.Rating
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<BookmarkDTO>> FilterBookmarksAsync(string courseName, int? courseId)
        {
            var query = _db.Bookmarks.Include(b => b.Course).AsQueryable();

            if (!string.IsNullOrEmpty(courseName))
            {
                query = query.Where(b => b.Course.Name.Contains(courseName));
            }

            if (courseId.HasValue)
            {
                query = query.Where(b => b.CourseId == courseId.Value);
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

        public async Task<IEnumerable<BookmarkDTO>> SortBookmarksAsync(string userId, string sortBy)
        {
            var query = _db.Bookmarks
                .Include(b => b.Course)
                .Where(b => b.UserId == userId)
                .AsQueryable();

            switch (sortBy.ToLower())
            {
                case "coursename":
                    query = query.OrderBy(b => b.Course.Name);
                    break;
                case "price":
                    query = query.OrderBy(b => b.Course.Price);
                    break;
                case "rating":
                    query = query.OrderBy(b => b.Course.Rating);
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

        public async Task<CourseDetailDTO> GetCourseDetailsAsync(string userId, int courseId)
        {
            var course = await _db.Courses
                .Include(c => c.Steps)
                .Include(c => c.Instructor)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null) return null;

            return new CourseDetailDTO
            {
                Id = course.Id,
                Name = course.Name,
                Content = course.Description,
                Instructor = new List<string> { course.Instructor?.UserName ?? "Unknown" },
                Steps = course.Steps.Select(step => step.Name).ToList()
            };
        }
    }
}
