using Cursus.Data.DTO;
using Cursus.Data.DTO.Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cursus.Service
{
    public class BookmarkService : IBookmarkService
    {
        private readonly IBookmarkRepository _bookmarkRepository;

        public BookmarkService(IBookmarkRepository bookmarkRepository)
        {
            _bookmarkRepository = bookmarkRepository;
        }

        public async Task<IEnumerable<BookmarkDTO>> GetBookmarksByUserIdAsync(string userId)
        {
            return await _bookmarkRepository.GetBookmarksByUserIdAsync(userId);
        }

        public async Task CreateBookmarkAsync(BookmarkCreateDTO bookmarkCreateDTO)
        {
            var bookmark = new Bookmark
            {
                UserId = bookmarkCreateDTO.UserId,
                CourseId = bookmarkCreateDTO.CourseId
            };
            await _bookmarkRepository.AddAsync(bookmark);
        }

        public async Task<IEnumerable<BookmarkDTO>> FilterBookmarksAsync(string courseName, int? courseId)
        {
            return await _bookmarkRepository.FilterBookmarksAsync(courseName, courseId);
        }

        public async Task<CourseDetailDTO> GetCourseDetailsAsync(string userId, int courseId)
        {
            return await _bookmarkRepository.GetCourseDetailsAsync(userId, courseId);
        }
    }
}
