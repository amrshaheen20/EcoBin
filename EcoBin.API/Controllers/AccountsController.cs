using EcoBin.API.Common;
using EcoBin.API.Extensions;
using EcoBin.API.Models.Dtos;
using EcoBin.API.Services.AccountContainer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoBin.API.Controllers
{
    /// <summary>
    /// For Admin Only
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Policies.Manger)]
    public class AccountsController(AccountService accountService) : BaseController
    {
        ///// <summary>          
        ///// Create a new account - Manger Only      
        ///// </summary>
        ///// <param name="account">The account to create</param>
        ///// <returns>The created account</returns>
        //[HttpPost]
        //[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AccountResponseDto))]
        //public async Task<IActionResult> Create([FromBody] AccountRequestDto account)
        //{
        //    return BuildResponse(await accountService.CreateAccountAsync(account));
        //}

        /// <summary>
        /// Get an account by ID - Manger Only
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountResponseDto))]
        public async Task<IActionResult> GetUsers([FromRoute] int id)
        {
            return BuildResponse(await accountService.GetAccountByIdAsync(id));
        }

        /// <summary>
        /// Get all accounts - Manger Only
        /// </summary>
        /// <param name="filter">The filter to apply to the accounts</param>
        /// <returns>The accounts</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginateBlock<AccountResponseDto>))]
        public IActionResult GetUsers([FromQuery] PaginationFilter<AccountResponseDto> filter)
        {
            return BuildResponse(accountService.GetAllAccounts(filter));
        }

        /// <summary>
        /// Update an account by ID - Manger Only
        /// </summary>
        /// <param name="id">The ID of the account to update</param>
        /// <param name="account">The account to update</param>
        /// <returns>The updated account</returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(AccountResponseDto))]
        public async Task<IActionResult> UpdateUser(int id, AccountRequestDto account)
        {
            return BuildResponse(await accountService.UpdateAccountAsync(id, account));
        }

        /// <summary>
        /// Delete an account by ID - Manger Only
        /// </summary>
        /// <param name="id">The ID of the account to delete</param>
        /// <returns>The deleted account</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(AccountResponseDto))]
        public async Task<IActionResult> DeleteUser(int id)
        {
            return BuildResponse(await accountService.DeleteAccountByIdAsync(id));
        }


    }
}

