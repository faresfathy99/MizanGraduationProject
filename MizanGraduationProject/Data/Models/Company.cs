namespace MizanGraduationProject.Data.Models
{
    public class Company
    {
        public string Id { get; set; }=Guid.NewGuid().ToString();
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
