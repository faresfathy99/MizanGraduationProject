using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MizanGraduationProject.Data.Models.ModelsConfigurations
{
    public class RatingConfiguration : IEntityTypeConfiguration<Rating>
    {
        public void Configure(EntityTypeBuilder<Rating> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasOne(e => e.User).WithMany(e => e.Ratings).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(e => e.Lawyer).WithMany(e => e.Ratings).HasForeignKey(e => e.LawyerId).OnDelete(DeleteBehavior.NoAction);
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(e => e.UpdatedAt).HasDefaultValueSql("getdate()");
            builder.Property(e => e.Rate).IsRequired().HasColumnName("Rate").HasMaxLength(10);
        }
    }
}
