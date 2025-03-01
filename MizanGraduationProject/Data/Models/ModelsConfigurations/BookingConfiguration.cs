using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MizanGraduationProject.Data.Models.ModelsConfigurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(e=>e.User).WithMany(e=>e.Bookings).HasForeignKey(e=>e.UserId);
            builder.HasOne(e=>e.Lawyer).WithMany(e=>e.Bookings).HasForeignKey(e=>e.LawyerId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(e=>e.Service).WithMany(e=>e.Bookings).HasForeignKey(e=>e.ServiceId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(e=>e.BookingStatus).WithMany(e=>e.Bookings).HasForeignKey(e=>e.StatusId);
            builder.Property(e => e.BookingDate).HasDefaultValueSql("getdate()");
            builder.Property(e => e.CreatedAt).HasDefaultValueSql("getdate()");
            builder.Property(e => e.UpdatedAt).HasDefaultValueSql("getdate()");
        }
    }
}
