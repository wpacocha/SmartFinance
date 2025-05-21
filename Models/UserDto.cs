using System.ComponentModel.DataAnnotations;

namespace SmartFinance.API.Models
{
    public class UserDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters long.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).+$",
            ErrorMessage = "Password must contain an uppercase letter, a lowercase letter and a special character.")]
        public string Password { get; set; } = string.Empty;
    }
}
