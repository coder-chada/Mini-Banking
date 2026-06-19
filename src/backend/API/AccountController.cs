using ApplicationService.Accounts.Contracts;
using ApplicationService.Accounts.DTOs;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace API
{
    [ApiController]
    [ApiVersion(1)]
    [Route("api/v{v:apiversion}/[controller]")]
    
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            this._accountService = accountService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAccountDTORequest request, CancellationToken cancellationToken)
        {
            var response = await _accountService
                .CreateAccountAsync(request, cancellationToken)
                .ConfigureAwait(false);

            return Ok(response);
        }
    }
}
