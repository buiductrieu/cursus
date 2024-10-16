using Cursus.RepositoryContract.Interfaces;
using Cursus.Common.Helper;
using Cursus.Data.DTO;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Cursus.Service.Services;
using Humanizer;
using System.Net;

namespace Cursus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReasonController : ControllerBase
    {
        private readonly IReasonService _reasonService;
        private readonly APIResponse _response;

        public ReasonController(IReasonService reasonService, APIResponse response)
        {
            _reasonService = reasonService;
            _response = response;
        }

        /// <summary>
        /// Create reason
        /// </summary>
        /// <param name="createReasonDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CreateReason([FromBody] CreateReasonDTO createReasonDTO)
        {
            var reason = await _reasonService.CreateReason(createReasonDTO);
            _response.Result = reason;
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpGet("{reasonId}")]
        public async Task<ActionResult<IEnumerable<ReasonDTO>>> GetReason (int reasonId)
        {
            var reason = await _reasonService.GetReasonByIdAsync(reasonId);
            return Ok(reason);
        }

    }
}
