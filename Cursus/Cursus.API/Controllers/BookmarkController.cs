using Cursus.Data.DTO;
using Cursus.Data.DTO.Cursus.Data.DTO;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cursus.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookmarkController : ControllerBase
    {
        private readonly IBookmarkService _bookmarkService;

        public BookmarkController(IBookmarkService bookmarkService)
        {
            _bookmarkService = bookmarkService;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<BookmarkDTO>>> GetBookmarksByUserId(string userId)
        {
            var bookmarks = await _bookmarkService.GetBookmarksByUserIdAsync(userId);
            return Ok(bookmarks);
        }

        [HttpPost]
        public async Task<ActionResult> CreateBookmark(BookmarkCreateDTO bookmarkCreateDTO)
        {
            await _bookmarkService.CreateBookmarkAsync(bookmarkCreateDTO);
            return NoContent();
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<BookmarkDTO>>> FilterBookmarks(string courseName = null, int? courseId = null)
        {
            var bookmarks = await _bookmarkService.FilterBookmarksAsync(courseName, courseId);
            return Ok(bookmarks);
        }

        //[HttpGet("sort")]
        //public async Task<ActionResult<IEnumerable<BookmarkDTO>>> SortBookmarks(string sortBy)
        //{
        //    var sortedBookmarks = await _bookmarkService.SortBookmarksAsync(sortBy);
        //    return Ok(sortedBookmarks);
        //}

        [HttpGet("{userId}/details/{courseId}")]
        public async Task<ActionResult<CourseDetailDTO>> GetCourseDetails(string userId, int courseId)
        {
            var courseDetails = await _bookmarkService.GetCourseDetailsAsync(userId, courseId);
            return Ok(courseDetails);
        }
    }
}
