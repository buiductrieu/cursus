using Cursus.Common.Helper;
using Cursus.Data.DTO;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Cursus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseCommentController : ControllerBase
    {
        private readonly ICourseCommentService _courseCommentService;
        private readonly APIResponse _response;

        public CourseCommentController(ICourseCommentService courseCommentService, APIResponse response)
        {
            _courseCommentService = courseCommentService;
            _response = response;
        }

        [HttpPost("post-comment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> PostComment([FromBody] CourseCommentCreateDTO dto)
        {

            var comment = await _courseCommentService.PostComment(dto);
            _response.Result = comment;
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);

        }
    }
}
