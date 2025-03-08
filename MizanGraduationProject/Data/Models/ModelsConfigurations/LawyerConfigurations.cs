using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MizanGraduationProject.Data.Models.ModelsConfigurations
{
    public class LawyerConfigurations : IEntityTypeConfiguration<Lawyer>
    {
        public void Configure(EntityTypeBuilder<Lawyer> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasOne(e => e.User).WithOne(e => e.Lawyer).HasForeignKey<Lawyer>(e => e.UserId);
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(e => e.UpdatedAt).HasDefaultValueSql("getdate()");
            builder.HasOne(e => e.Specialization).WithMany(e => e.Lawyers).HasForeignKey(e => e.SpecializationId);
        }
    }
}
