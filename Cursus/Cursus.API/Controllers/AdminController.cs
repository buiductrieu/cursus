using Cursus.Common.Helper;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Cursus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
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

            try
            {
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
            }
            catch (Exception ex)
            {
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.IsSuccess = false;
                apiResponse.ErrorMessages.Add(ex.Message);
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
    }
}
