using ApplicationService.BankTransactions.Contracts;
using ApplicationService.BankTransactions.DTOs;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion(1)]
    [Route("api/v{v:apiversion}/[controller]")]
    public class BankTransactionsController : ControllerBase
    {
        private readonly IBankTransactionService _bankTransaction;

        public BankTransactionsController(IBankTransactionService bankTransaction)
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

        [HttpPost("withdrawal")]
        public async Task<IActionResult> Withdrawal(
            [FromBody] CreateWithdrawalRequest request,
            CancellationToken cancellationToken
        )
        {
            var key = HttpContext.Items["IdempotencyKey"]?.ToString();
            var requestHash = HttpContext.Items["RequestHash"]?.ToString();

            var result = await _bankTransaction.MakeWithdrawalAsync(
                idempotencyKey: key ?? string.Empty,
                requestHash: requestHash ?? string.Empty,
                withdrawalDTO: request,
                cancellationToken: cancellationToken
            );

            return Ok(result);
        }

        [HttpPatch("transfer")]
        public async Task<IActionResult> Transfer(
            [FromBody] CreateTransferRequest request,
            CancellationToken cancellationToken
        )
        {
            var key = HttpContext.Items["IdempotencyKey"]?.ToString();
            var requestHash = HttpContext.Items["RequestHash"]?.ToString();

            var result = await _bankTransaction.MakeTransferAsync(
                idempotencyKey: key ?? string.Empty,
                requestHash: requestHash ?? string.Empty,
                transferDTO: request,
                cancellationToken: cancellationToken
            );

            return Ok(result);
        }
    }
}
