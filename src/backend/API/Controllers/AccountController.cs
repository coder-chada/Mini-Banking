using ApplicationService.Accounts.Contracts;
using ApplicationService.Accounts.DTOs;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion(1)]
    [Route("api/v{v:apiversion}/[Controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            this._accountService = accountService;
        }

        [HttpPost]
        public async Task<IActionResult> Add(CreateAccountDTORequest request, CancellationToken token)
        {
            var response = await _accountService
                .CreateAccountAsync(accountDTO: request, cancellationToken: token)
                .ConfigureAwait(false);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken token)
        {
            var response = await _accountService
                .GetAccountByAsync(ID: id, cancellationToken: token)
                .ConfigureAwait(false);

            return Ok(response);
        }
    }
}
