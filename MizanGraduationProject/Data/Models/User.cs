using Microsoft.AspNetCore.Identity;
using MizanGraduationProject.Data.Models.OTP;

namespace MizanGraduationProject.Data.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
       
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Lawyer Lawyer { get; set; } = null!;
        public List<Review> Reviews { get; set; } = null!;
        public List<Booking> Bookings { get; set; } = null!;
        public List<Rating> Ratings { get; set; } = null!;
        public List<LoginOTP> LoginOTPs { get; set; } = null!;
        public List<VerifyOTP> VerifyOTPs { get; set; } = null!;
    }
}
