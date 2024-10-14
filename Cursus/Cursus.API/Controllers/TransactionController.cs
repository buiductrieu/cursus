using Cursus.Common.Helper;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Cursus.API.Controllers
{
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

       
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTransactionsByUserId(string userId, int page = 1, int pageSize = 20)
        {
            var transactions = await _transactionService.GetListTransactionByUserId(userId, page, pageSize);
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = transactions;
            return Ok(_response);
        }

    }
}
