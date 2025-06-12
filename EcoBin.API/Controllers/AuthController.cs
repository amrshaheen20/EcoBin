using EcoBin.API.Extensions;
using EcoBin.API.Models.Dtos;
using EcoBin.API.Services.AccountContainer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcoBin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(AccountService accountService) : BaseController
    {
        /// <summary>
        /// Login - Anonymous
        /// </summary>
        /// <param name="account">The account to login</param>
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseDto))]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequestDto account)
        {
            return BuildResponse(await accountService.LoginAsync(account));
        }

        /// <summary>
        /// Register - Anonymous
        /// </summary>
        /// <param name="account">The account to register</param>
        [HttpPost("Register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Register(RegisterRequestDto account)
        {
            return BuildResponse(await accountService.RegisterAsync(account));
        }

        /// <summary>
        /// Forgot Password - Anonymous
        /// </summary>
        /// <param name="request">The email or identifier for password reset</param>
        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ForgotPasswordRequestDto))]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDto request)
        {
            return BuildResponse(await accountService.ForgotPasswordAsync(request));
        }


        ///<summary>
        /// Verify Reset Password Token - Anonymous
        /// </summary>
        /// <param name="request"> The request containing the email and token to verify</param>
        [HttpPost("verify-reset-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyResetToken([FromBody] VerifyResetTokenRequestDto request)
        {
            return BuildResponse(await accountService.VerifyResetTokenAsync(request));
        }

        /// <summary>
        /// Change Password - Anonymous
        /// </summary>
        /// <param name="request"></param>
        [HttpPut("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseDto))]
        [AllowAnonymous] // This should probably be [Authorize] instead
        public async Task<IActionResult> ChangePassword(ResetPasswordRequestDto request)
        {
            return BuildResponse(await accountService.ChangePasswordAsync(request));
        }

        /// <summary>
        /// Logout - Authenticated
        /// </summary>
        [HttpPost("Logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Policy = Policies.AllUsers)]
        public async Task<IActionResult> Logout()
        {
            return BuildResponse(await accountService.Logout());
        }
    }
}
