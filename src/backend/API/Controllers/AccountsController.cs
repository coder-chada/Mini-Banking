using ApplicationService.Accounts.Contracts;
using ApplicationService.Accounts.DTOs;
using Asp.Versioning;
using DomainLogic.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion(1)]
    [Route("api/v{v:apiversion}/[Controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            this._accountService = accountService;
        }

        [HttpPost]
        public async Task<IActionResult> Add(CreateAccountRequest request, CancellationToken token)
        {
            var response = await _accountService
                .CreateAccountAsync(accountDTO: request, cancellationToken: token)
                .ConfigureAwait(false);

            return Ok(response);
        }

        [HttpGet("{accountID:int}")]
        public async Task<IActionResult> Get(int accountID, CancellationToken token)
        {
            var response = await _accountService
                .GetAccountByAsync(accountID: accountID, cancellationToken: token)
                .ConfigureAwait(false);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetByUserID([FromQuery] int userID, CancellationToken token)
        {
            var response = await _accountService
                .GetAccountsByAsync(userID: new UserID(userID), cancellationToken: token)
                .ConfigureAwait(false);

            return Ok(response);
        }
    }
}
