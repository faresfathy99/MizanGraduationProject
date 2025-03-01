using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MizanGraduationProject.Data.Models.ModelsConfigurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Email).IsRequired();
            builder.Property(x => x.Phone).IsRequired();
            builder.Property(x => x.Address).IsRequired();
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(e => e.UpdatedAt).HasDefaultValueSql("getdate()");
        }
    }
}
