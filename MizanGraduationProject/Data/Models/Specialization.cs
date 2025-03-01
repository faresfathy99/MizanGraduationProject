namespace MizanGraduationProject.Data.Models
{
    public class Specialization
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<Lawyer> Lawyers { get; set; } = null!;
    }
}
