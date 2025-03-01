using System.ComponentModel.DataAnnotations;

namespace MizanGraduationProject.Data.DTOs
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Please Enter your First Name")]
        [Length(3, 20, ErrorMessage = "First Name Length must be at least 3 chars and maxium 20 chars")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter your Last Name")]
        [Length(3, 20, ErrorMessage = "Last Name Length must be at least 3 chars and maxium 20 chars")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter your Email")]
        [EmailAddress]
        [RegularExpression("^\\S+@\\S+\\.\\S+$")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password should be at least 8 digits containg lower, upper case characters and special characters")]
        public string Password { get; set; } = null!;
    }
}
