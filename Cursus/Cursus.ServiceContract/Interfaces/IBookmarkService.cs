using Cursus.Data.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cursus.ServiceContract.Interfaces
{
    public interface IBookmarkService
    {
        Task<IEnumerable<BookmarkDTO>> GetFilteredAndSortedBookmarksAsync(string userId, string? courseName, int? courseId, string? sortBy, string sortOrder);

        Task<CourseDetailDTO> GetCourseDetailsAsync(int courseId);

        Task CreateBookmarkAsync(BookmarkCreateDTO bookmarkCreateDTO);
    }
}
