using Cursus.Data.Entities;
using Cursus.Data.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cursus.Data.DTO.Cursus.Data.DTO;

namespace Cursus.RepositoryContract.Interfaces
{
    public interface IBookmarkRepository : IRepository<Bookmark>
    {
        Task<IEnumerable<BookmarkDTO>> GetBookmarksByUserIdAsync(string userId);
        Task<IEnumerable<BookmarkDTO>> FilterBookmarksAsync(string courseName, int? courseId);
        Task<CourseDetailDTO> GetCourseDetailsAsync(string userId, int courseId);
            
    }
}
