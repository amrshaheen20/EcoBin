using EcoBin.API.Enums;
using EcoBin.API.Validators;
using System.ComponentModel.DataAnnotations;

namespace EcoBin.API.Models.Dtos
{

    public class RegisterRequestDto
    {
        [IsValid(typeof(RequiredAttribute))]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string? Email { get; set; }

        [IsValid(typeof(RequiredAttribute))]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
        public string? Password { get; set; }

        [IsValid(typeof(RequiredAttribute))]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
        public string? Name { get; set; }

        /// <example>01012345678</example>
        [RegularExpression(@"^01[0-9]{9}$", ErrorMessage = "Invalid Phone Number, it should be in this format: 01XXXXXXXXX")]
        public string? PhoneNumber { get; set; }

        [IsValid(typeof(RequiredAttribute))]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "Company Name must be between 3 and 100 characters.")]
        public string? CompanyName { get; set; }

    }

    public class AccountRequestDto : RegisterRequestDto
    {
        [IsValid(typeof(RequiredAttribute))]
        public eRole? Role { get; set; }
    }

    public class AccountResponseDto : BaseResponseDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public eRole? Role { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CompanyName { get; set; }
        public DateTime LastActiveTime { get; set; }
    }

}
