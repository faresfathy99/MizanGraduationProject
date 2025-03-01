using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MizanGraduationProject.Data.Models.ModelsConfigurations
{
    public class ReviewConfiguration: IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasOne(e => e.User).WithMany(e => e.Reviews).HasForeignKey(e => e.UserId);
            builder.HasOne(e => e.Lawyer).WithMany(e => e.Reviews).HasForeignKey(e => e.LawyerId).OnDelete(DeleteBehavior.NoAction);
            builder.Property(e=>e.UserId).IsRequired();
            builder.Property(e=>e.LawyerId).IsRequired();
            builder.Property(e=>e.Comment).IsRequired();
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(e => e.UpdatedAt).HasDefaultValueSql("getdate()");
        }
    }
}
