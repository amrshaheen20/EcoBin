using System.ComponentModel.DataAnnotations;

namespace EcoBin.API.Models.Dtos
{
    public class LoginRequestDto
    {
        [EmailAddress]
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class LoginResponseDto
    {
        public required string Token { get; set; }
        public required DateTime Expiration { get; set; }
        public required AccountResponseDto User { get; set; } = default!;
    }


    public class VerifyResetTokenRequestDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = default!;
        [Required]
        public string Token { get; set; } = default!;
    }

    public class ResetPasswordRequestDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = default!;

        [Required]
        public string Token { get; set; } = default!;

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
        public string NewPassword { get; set; } = default!;
    }

    public class ForgotPasswordRequestDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = default!;
    }




}
