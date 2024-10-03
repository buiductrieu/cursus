using Cursus.Common.Helper;
using Cursus.Data.DTO;
using Cursus.Data.Entities;
using Cursus.RepositoryContract.Interfaces;
using Cursus.Service.Services;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Cursus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly APIResponse _response;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;


        public AuthController(IAuthService authService, APIResponse response, UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _authService = authService;
            _response = response;
            _userManager = userManager;
            _emailService = emailService;
        }

        /// <summary>
        /// Login for user
        /// </summary>
        /// <param name="LoginRequestDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            try
            {
                var responseDTO = await _authService.LoginAsync(loginRequestDTO);
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = responseDTO;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages.Add(ex.Message);
                return StatusCode(500, _response);

            }

        }

        /// <summary>
        /// Register for user
        /// </summary>
        /// <param name="UserRegisterDTO"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> Register(UserRegisterDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Result = ModelState;
            }
            var result = await _authService.RegisterAsync(dto);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(result);

            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Auth", new { token = token, username = result.UserName }, Request.Scheme);

            _emailService.SendEmailConfirmation(result.UserName, confirmationLink);

            _response.IsSuccess = true;

            _response.StatusCode = HttpStatusCode.OK;

            _response.Result = result;

            return Ok(_response);

        }

        /// <summary>
        /// Confirm email
        /// </summary>
        /// <param name="token"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet("confirm-email")]
        public async Task<ActionResult<APIResponse>> ConfirmEmail([FromQuery] string token, [FromQuery] string username)
        {
            try
            {
                var result = await _authService.ConfirmEmail(username, token);
                if (result)
                {
                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.OK;
                    return Ok(_response);
                }
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Can not confirm your email");
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(e.Message);
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
        }
    }

}
