using System.ComponentModel.DataAnnotations;

namespace MizanGraduationProject.Data.DTOs
{
    public class EnableDisable2fa
    {
        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "Please enter valid email address")]
        public string Email { get; set; } = null!;
    }
}
