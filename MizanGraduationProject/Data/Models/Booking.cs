namespace MizanGraduationProject.Data.Models
{
    public class Booking
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = null!;
        public string LawyerId { get; set; } = null!;
        public string ServiceId { get; set; } = null!;
        public string StatusId { get; set; } = null!;
        public DateTime BookingDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Lawyer Lawyer { get; set; } = null!;
        public User User { get; set; } = null!;
        public Service Service { get; set; } = null!;
        public BookingStatus BookingStatus { get; set; } = null!;
    }
}
