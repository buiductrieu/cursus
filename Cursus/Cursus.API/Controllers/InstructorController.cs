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
            try
            {
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
            catch (System.Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages.Add(ex.Message);
                return StatusCode(500, _response);
            }
        }
    }
}
