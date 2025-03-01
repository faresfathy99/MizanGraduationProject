namespace MizanGraduationProject.Data.Models
{
    public class BookingStatus
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<Booking> Bookings { get; set; } = null!;
    }
}
