using Cursus.Data.DTO;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cursus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookmarkController : ControllerBase
    {
        private readonly IBookmarkService _bookmarkService;

        public BookmarkController(IBookmarkService bookmarkService)
        {
            _bookmarkService = bookmarkService;
        }
        /// <summary>
        /// GetBookMarks
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        // Get bookmarks with sorting functionality
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookmarkDTO>>> GetBookmarks(
            string userId,
            string? sortBy = null,
            string? sortOrder = "asc")
        {
            var bookmarks = await _bookmarkService.GetFilteredAndSortedBookmarksAsync(userId, null, null, sortBy, sortOrder);
            return Ok(bookmarks);
        }
        /// <summary>
        /// GetCourseDetails
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        [HttpGet("{courseId}/details")]
        public async Task<ActionResult<CourseDetailDTO>> GetCourseDetails(int courseId)
        {
            var courseDetails = await _bookmarkService.GetCourseDetailsAsync(courseId);
            if (courseDetails == null) return NotFound();
            return Ok(courseDetails);
        }
        /// <summary>
        /// CreateBookMark
        /// </summary>
        /// <param name="bookmarkCreateDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateBookmark([FromBody] BookmarkCreateDTO bookmarkCreateDTO)
        {
            await _bookmarkService.CreateBookmarkAsync(bookmarkCreateDTO);
            return CreatedAtAction(nameof(GetBookmarks), new { userId = bookmarkCreateDTO.UserId }, bookmarkCreateDTO);
        }
    }
}
