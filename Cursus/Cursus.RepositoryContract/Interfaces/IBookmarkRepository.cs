using Cursus.Data.DTO;
using Cursus.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cursus.RepositoryContract.Interfaces
{
    public interface IBookmarkRepository
    {
        Task<IEnumerable<BookmarkDTO>> GetFilteredAndSortedBookmarksAsync(string userId, string? courseName, int? courseId, string? sortBy, string sortOrder);

        Task<CourseDetailDTO> GetCourseDetailsAsync(int courseId);

        Task AddAsync(Bookmark bookmark);
    }
}
