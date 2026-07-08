using ApplicationService.Users.Contracts;
using ApplicationService.Users.DTOs;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion(1)]
    [Route("api/v{v:apiversion}/[Controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            this._userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserRequest request,
                                                CancellationToken cancellationToken)
        {
            var response = await _userService
                .CreateUserAsync(request,
                                 cancellationToken)
                .ConfigureAwait(false);

            return Ok(response);
        }
    }
}
