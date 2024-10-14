using AutoMapper;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookmarkService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookmarkDTO>> GetFilteredAndSortedBookmarksAsync(string userId, string? courseName, int? courseId, string? sortBy, string sortOrder)
        {
            var bookmarks = await _unitOfWork.BookmarkRepository.GetFilteredAndSortedBookmarksAsync(userId, courseName, courseId, sortBy, sortOrder);
            return _mapper.Map<IEnumerable<BookmarkDTO>>(bookmarks);
        }

        public async Task<CourseDetailDTO> GetCourseDetailsAsync(int courseId)
        {
            var courseDetails = await _unitOfWork.BookmarkRepository.GetCourseDetailsAsync(courseId);
            return _mapper.Map<CourseDetailDTO>(courseDetails);
        }

        public async Task CreateBookmarkAsync(BookmarkCreateDTO bookmarkCreateDTO)
        {
            var bookmark = new Bookmark
            {
                UserId = bookmarkCreateDTO.UserId,
                CourseId = bookmarkCreateDTO.CourseId,
                DateCreated = System.DateTime.UtcNow
            };

            await _unitOfWork.BookmarkRepository.AddAsync(bookmark);
            await _unitOfWork.SaveChanges(); // Save changes through UnitOfWork
        }
    }
}
