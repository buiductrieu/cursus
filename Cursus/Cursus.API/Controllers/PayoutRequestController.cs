using Cursus.Common.Helper;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Cursus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayoutRequestController : ControllerBase
    {
        public readonly IPayoutRequestService _payoutRequestService;
        public readonly APIResponse _response;

        public PayoutRequestController(IPayoutRequestService payoutRequestService)
        {
            _payoutRequestService = payoutRequestService;
            _response = new APIResponse();
        }

        [HttpGet("pending")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> GetPendingPayoutRequests()
        {
            var pendingRequests = await _payoutRequestService.GetPendingPayoutRequest();
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = pendingRequests;
            return Ok(_response);
        }


        [HttpGet("approved")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> GetApprovedPayoutRequest()
        {
            var approvedRequest = await _payoutRequestService.GetApprovedPayoutRequest();
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = approvedRequest;
            return Ok(_response);
        }

        [HttpGet("reject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> GetRejectedPayoutRequest()
        {
            var rejectRequest = await _payoutRequestService.GetRejectPayoutRequest();
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = rejectRequest;
            return Ok(_response);
        }
    }
}
