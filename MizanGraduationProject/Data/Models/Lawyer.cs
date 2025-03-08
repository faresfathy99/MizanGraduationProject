namespace MizanGraduationProject.Data.Models
{
    public class Lawyer
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = null!;
        public string SpecializationId { get; set; } = null!;
        public DateTime StartedAt { get; set; }
        public string Location { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public User User { get; set; } = null!;
        public Specialization Specialization { get; set; } = null!;
        public List<Review> Reviews { get; set; } = null!;
        public List<Service> Services { get; set; } = null!;
        public List<Booking> Bookings { get; set; } = null!;
        public List<Rating> Ratings { get; set; } = null!;

    }
}
