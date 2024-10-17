using Cursus.Common.Helper;
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
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly APIResponse _response;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
            _response = new APIResponse();
        }
        /// <summary>
        /// Modify user's status
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        // POST api/admin/toggleuserstatus?userId=someUserId
        [HttpPost("toggle-user-status")]
        public async Task<IActionResult> ToggleUserStatus(string userId)
        {
            var apiResponse = new APIResponse();

            var result = await _adminService.ToggleUserStatusAsync(userId);
            if (result)
            {
                apiResponse.StatusCode = HttpStatusCode.OK;
                apiResponse.IsSuccess = true;
                apiResponse.Result = "User status has been updated";
            }
            else
            {
                apiResponse.StatusCode = HttpStatusCode.BadRequest;
                apiResponse.IsSuccess = false;
                apiResponse.ErrorMessages.Add("Failed to update user status");
            }
            return StatusCode((int)apiResponse.StatusCode, apiResponse);
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns></returns>
        // GET api/admin/manageusers
        [HttpGet("manage-users")]
        public async Task<IActionResult> ManageUsers()
        {
            var apiResponse = new APIResponse();

            try
            {
                var users = await _adminService.GetAllUser();
                if (users != null && users.Count() > 0)
                {
                    apiResponse.StatusCode = HttpStatusCode.OK;
                    apiResponse.IsSuccess = true;
                    apiResponse.Result = users;
                }
                else
                {
                    apiResponse.StatusCode = HttpStatusCode.NotFound;
                    apiResponse.IsSuccess = false;
                    apiResponse.ErrorMessages.Add("No users found");
                }
            }
            catch (Exception ex)
            {
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.IsSuccess = false;
                apiResponse.ErrorMessages.Add(ex.Message);
            }

            return StatusCode((int)apiResponse.StatusCode, apiResponse);

        }

        [HttpPost("add-comments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AdminComments([FromQuery]string userId,[FromQuery] string comment)
        {
          
            var result = await _adminService.AdminComments(userId, comment);
            if (result)
            {
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = "Comment is sucessful";
                return Ok(_response);
            }
            else
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Failed to add comment");
                return BadRequest(_response);
            }
        }
        [HttpGet("get-instructor-info")]
        public async Task<IActionResult> GetInformationInstructor([FromQuery] int instructorId)
        {
            var apiResponse = new APIResponse();

            // Lấy thông tin của instructor thông qua service
            var instructorInfo = await _adminService.GetInformationInstructor(instructorId);

            if (instructorInfo != null && instructorInfo.Count > 0)
            {
                apiResponse.StatusCode = HttpStatusCode.OK;
                apiResponse.IsSuccess = true;
                apiResponse.Result = new
                {
                    UserName = instructorInfo.ContainsKey("UserName") ? instructorInfo["UserName"] : null,
                    Email = instructorInfo.ContainsKey("Email") ? instructorInfo["Email"] : null,
                    PhoneNumber = instructorInfo.ContainsKey("PhoneNumber") ? instructorInfo["PhoneNumber"] : null,
                    TotalCourses = instructorInfo.ContainsKey("TotalCourses") ? instructorInfo["TotalCourses"] : 0,
                    TotalActiveCourses = instructorInfo.ContainsKey("TotalActiveCourses") ? instructorInfo["TotalActiveCourses"] : 0,
                    TotalEarning = instructorInfo.ContainsKey("TotalEarning") ? instructorInfo["TotalEarning"] : 0.0,
                    TotalPayout = instructorInfo.ContainsKey("TotalPayout") ? instructorInfo["TotalPayout"] : 0.0,
                    AverageRating = instructorInfo.ContainsKey("AverageRating") ? instructorInfo["AverageRating"] : 0.0,
                    AdminComment = instructorInfo.ContainsKey("AdminComment") ? instructorInfo["AdminComment"] : null,
                };
            }
            else
            {
                apiResponse.StatusCode = HttpStatusCode.NotFound;
                apiResponse.IsSuccess = false;
                apiResponse.ErrorMessages.Add("Instructor not found");
            }

            return StatusCode((int)apiResponse.StatusCode, apiResponse);
        }



    }
}

