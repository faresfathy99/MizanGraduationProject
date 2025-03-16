using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MizanGraduationProject.Data.Models.ModelsConfigurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.NormalizedName).IsRequired();
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(e => e.UpdatedAt).HasDefaultValueSql("getdate()");
            builder.HasIndex(e => new { e.Name }).IsUnique();
            builder.HasIndex(e => new { e.NormalizedName }).IsUnique();
        }
    }
}
