﻿using Cursus.Data.DTO;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cursus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StepCommentController : ControllerBase
    {
        private readonly IStepCommentService _stepCommentService;

        public StepCommentController(IStepCommentService stepCommentService)
        {
            _stepCommentService = stepCommentService;
        }
        /// <summary>
        /// Post-Comment
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="BadHttpRequestException"></exception>
        [HttpPost("post-comment")]
        public async Task<ActionResult<StepCommentDTO>> PostComment([FromBody] StepCommentCreateDTO dto)
        {
            if (dto == null) throw new BadHttpRequestException("Comment data is required.");

            var comment = await _stepCommentService.PostStepComment(dto);
            return Ok(comment);
        }
        /// <summary>
        /// Get-Comments
        /// </summary>
        /// <param name="stepId"></param>
        /// <returns></returns>
        [HttpGet("{stepId}/comments")]
        public async Task<ActionResult<IEnumerable<StepCommentDTO>>> GetComments(int stepId)
        {
            var comments = await _stepCommentService.GetStepCommentsAsync(stepId);
            return Ok(comments);
        }
        /// <summary>
        /// Delete-Comment
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [HttpDelete("{commentId}")]
        public async Task<ActionResult> DeleteComment(int commentId)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isDeleted = await _stepCommentService.DeleteStepCommentIfAdmin(commentId, adminId);
    //        return Ok("Comment deleted successfully.");
            return NoContent();
        }
    }
}