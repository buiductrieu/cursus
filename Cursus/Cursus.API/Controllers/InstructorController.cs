using Cursus.Common.Helper;
using Cursus.Data.DTO;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Net;

namespace Cursus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("default")]
    public class InstructorController : ControllerBase
    {
        private readonly IInstructorService _instructorService;
        private readonly APIResponse _response;
        public InstructorController(IInstructorService instructorService , APIResponse aPIResponse)
        {
            _instructorService = instructorService;
            _response = aPIResponse;
        }

        /// <summary>
        /// Register for instructor
        /// </summary>
        /// <param name="registerInstructorDTO"></param>
        /// <returns></returns>
        [HttpPost("register-instructor")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> RegisterInstructor (RegisterInstructorDTO registerInstructorDTO)
        {
            if (!ModelState.IsValid)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Result = ModelState;
                return BadRequest(_response);
            }

            var result = await _instructorService.InstructorAsync(registerInstructorDTO);

            if (result.Succeeded)
            {
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.Created;
                _response.Result = "Instructor registered successfully";
                return CreatedAtAction(nameof(RegisterInstructor), _response);
            }

            _response.IsSuccess = false;
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.ErrorMessages = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(_response);
        }


        [HttpPost("approve")]
        public async Task<ActionResult<APIResponse>> ApproveInstructor([FromQuery] string instructorId)
        {
            var result = await _instructorService.ApproveInstructorAsync(instructorId);
            if (result)
            {
                _response.IsSuccess = true;
                _response.Result = "Instructor approved successfully";
                return Ok(_response);
            }

            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Failed to approve instructor");
            return BadRequest(_response);
        }

        // API để từ chối tài khoản giảng viên
        [HttpPost("reject")]
        public async Task<ActionResult<APIResponse>> RejectInstructor([FromQuery] string instructorId)
        {
            var result = await _instructorService.RejectInstructorAsync(instructorId);
            if (result)
            {
                _response.IsSuccess = true;
                _response.Result = "Instructor rejected successfully";
                return Ok(_response);
            }

            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Failed to reject instructor");
            return BadRequest(_response);
        }
        /// <summary>
        /// List all instructors along with user and instructor information
        /// </summary>
        /// <returns></returns>
        [HttpGet("list-instructors")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetAllInstructors()
        {
            var instructors = await _instructorService.GetAllInstructors();
            if (instructors == null || !instructors.Any())
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add("No instructors found");
                return NotFound(_response);
            }

            var result = instructors.Select(instructor => new
            {
                UserId = instructor.User?.Id,
                UserName = instructor.User?.UserName,
                Email = instructor.User?.Email,
                PhoneNumber = instructor.User?.PhoneNumber,
                InstructorId = instructor.Id,
                CardName = instructor.CardName,
                CardProvider = instructor.CardProvider,
                CardNumber = instructor.CardNumber,
                SubmitCertificate = instructor.SubmitCertificate,
                StatusInstructor = instructor.StatusInsructor
            });

            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = result;

            return Ok(_response);
        }
    }
}
