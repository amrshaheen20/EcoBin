using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcoBin.API.Extensions;
using EcoBin.API.Models.Dtos;
using EcoBin.API.Services.AccountContainer;

namespace EcoBin.API.Controllers
{

    [Route("api/me")]
    [ApiController]
    [Authorize(Policy = Policies.AllUsers)]
    [ApiExplorerSettings(GroupName = "@me")]
    public class UserController(AccountService accountService) : BaseController
    {
        /// <summary>
        /// Get the current account - For All
        /// </summary>
        /// <returns>The current account</returns>
        [HttpGet("profile")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountResponseDto))]
        public IActionResult GetCurrentAccount() => BuildResponse(accountService.GetAccountByAuthAsync());

    }
}
