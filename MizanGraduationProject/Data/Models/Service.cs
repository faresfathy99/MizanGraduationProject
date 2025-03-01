namespace MizanGraduationProject.Data.Models
{
    public class Service
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string LawyerId { get; set; } = null!;
        public string ServiceName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Lawyer Lawyer { get; set; } = null!;
        public List<Booking> Bookings { get; set; } = null!;
    }
}
