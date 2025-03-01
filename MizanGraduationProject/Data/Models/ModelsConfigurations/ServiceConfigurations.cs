using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MizanGraduationProject.Data.Models.ModelsConfigurations
{
    public class ServiceConfigurations : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(e => e.Lawyer).WithMany(e => e.Services).HasForeignKey(e => e.LawyerId);
            builder.Property(e => e.LawyerId).IsRequired();
            builder.Property(e => e.Price).IsRequired().HasDefaultValue(0.0);
            builder.Property(e => e.ServiceName).IsRequired();
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(e => e.UpdatedAt).HasDefaultValueSql("getdate()");
        }
    }
}
