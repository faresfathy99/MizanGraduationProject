namespace MizanGraduationProject.Data.Models
{
    public class Location
    {
        public string Id { get; set; }=Guid.NewGuid().ToString();
        public string Name { get; set; } = null!;
        public string NormalizedName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
