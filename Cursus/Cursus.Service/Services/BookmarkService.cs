using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cursus.Service.Services
{
    public class BookmarkService : IBookmarkService
    {
        private readonly IBookmarkRepository _bookmarkRepository;

        public BookmarkService(IBookmarkRepository bookmarkRepository)
        {
            _bookmarkRepository = bookmarkRepository;
        }
        public async Task<IEnumerable<BookmarkDTO>> GetFilteredAndSortedBookmarksAsync(string userId, string? courseName, int? courseId, string? sortBy, string sortOrder)
        {
            return await _bookmarkRepository.GetFilteredAndSortedBookmarksAsync(userId, courseName, courseId, sortBy, sortOrder);
        }

        public async Task<CourseDetailDTO> GetCourseDetailsAsync(int courseId)
        {
            return await _bookmarkRepository.GetCourseDetailsAsync(courseId);
        }

        public async Task CreateBookmarkAsync(BookmarkCreateDTO bookmarkCreateDTO)
        {
            var bookmark = new Bookmark
            {
                UserId = bookmarkCreateDTO.UserId,
                CourseId = bookmarkCreateDTO.CourseId,
                DateCreated = System.DateTime.UtcNow
            };
            await _bookmarkRepository.AddAsync(bookmark);
        }
    }
}
