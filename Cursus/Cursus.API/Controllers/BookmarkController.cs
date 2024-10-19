using Cursus.Common.Helper;
using Cursus.Data.DTO;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Cursus.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class BookmarkController : ControllerBase
    {
        private readonly IBookmarkService _bookmarkService;
        private readonly APIResponse _response;

        public BookmarkController(IBookmarkService bookmarkService, APIResponse response)
        {
            _bookmarkService = bookmarkService;
            _response = response;
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
            var bookmarks = await _bookmarkService.GetFilteredAndSortedBookmarksAsync(userId, sortBy, sortOrder);
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
        public async Task<ActionResult<APIResponse>> CreateBookmark([FromBody] BookmarkCreateDTO bookmarkCreateDTO)
        {
            await _bookmarkService.CreateBookmarkAsync(bookmarkCreateDTO);

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = "Create successfully.";

            return Ok(_response);
        }
    }
}
