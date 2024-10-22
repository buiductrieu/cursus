namespace Demo_PayPal.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Cursus.Common.Helper;  // Use APIResponse from Helper
    using System.Net;
    using System.Threading.Tasks;
    using Cursus.ServiceContract.Interfaces;
    using Cursus.Data.DTO.Payment;

    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly APIResponse _response;

        public TransactionController(ITransactionService transactionService, APIResponse response)
        {
            _transactionService = transactionService;
            _response = response;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllTransactions(int page = 1, int pageSize = 20)
        {
            var transactions = await _transactionService.GetListTransaction(page, pageSize);
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = transactions;
            return Ok(_response);
        }

        /// <summary>
        /// Get transaction by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTransactionsByUserId(string userId, int page = 1, int pageSize = 20)
        {
            var transactions = await _transactionService.GetListTransactionByUserId(userId, page, pageSize);
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = transactions;
            return Ok(_response);
        }


        /// <summary>
        /// Get pending payout request
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-pending-payout")]
        public async Task<IActionResult> GetPendingPayoutRequest()
        {
            var transaction = await _transactionService.GetAllPendingPayOutRequest();
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = transaction;
            return Ok(_response);
        }
    }
}

