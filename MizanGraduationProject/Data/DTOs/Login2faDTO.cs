using System.ComponentModel.DataAnnotations;

namespace MizanGraduationProject.Data.DTOs
{
    public class Login2faDTO
    {
        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage ="Please enter valid email address")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Please enter 2fa token")]
        public string Token { get; set; } = null!;
    }
}
