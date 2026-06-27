using ApplicationService.BankTransactions.Contracts;
using ApplicationService.BankTransactions.DTOs;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion(1)]
    [Route("api/v{v:apiversion}/[controller]")]
    public class BankTransactionController : ControllerBase
    {
        private readonly IBankTransactionService _bankTransaction;

        public BankTransactionController(IBankTransactionService bankTransaction)
        {
            this._bankTransaction = bankTransaction;
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(
            [FromBody] CreateDepositRequest request,
            CancellationToken cancellationToken
        )
        {
            var key = HttpContext.Items["IdempotencyKey"]?.ToString();
            var requestHash = HttpContext.Items["RequestHash"]?.ToString();

            var result = await _bankTransaction.MakeDepositAsync(
                idempotencyKey: key ?? string.Empty,
                requestHash: requestHash ?? string.Empty,
                depositDTO: request,
                cancellationToken: cancellationToken
            );

            return Ok(result);
        }
    }
}
