using Cursus.Data.DTO;
using Cursus.Data.DTO.Cursus.Data.DTO;
using Cursus.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cursus.ServiceContract.Interfaces
{
    public interface IBookmarkService
    {
        Task<IEnumerable<BookmarkDTO>> GetBookmarksByUserIdAsync(string userId);
        Task CreateBookmarkAsync(BookmarkCreateDTO bookmarkCreateDTO);
        Task<IEnumerable<BookmarkDTO>> FilterBookmarksAsync(string courseName, int? courseId);
        Task<CourseDetailDTO> GetCourseDetailsAsync(string userId, int courseId);
    }
}
