using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MizanGraduationProject.Data.Models;
using MizanGraduationProject.Data.Models.ModelsConfigurations;

namespace MizanGraduationProject.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedRoles(builder);
            ApplyModelsConfigurations(builder);
        }

        private void SeedRoles(ModelBuilder builder)
        {
            //builder.Entity<IdentityRole>().HasData(
            //    new IdentityRole { ConcurrencyStamp = "1", Name = "ADMIN", NormalizedName = "ADMIN" },
            //    new IdentityRole { ConcurrencyStamp = "2", Name = "USER", NormalizedName = "USER" }
            //    );
            //builder.Entity<BookingStatus>().HasData(
            //    new BookingStatus { Status = "Pending", Id = Guid.NewGuid().ToString() },
            //    new BookingStatus { Status = "Approved", Id = Guid.NewGuid().ToString() },
            //    new BookingStatus { Status = "Rejected", Id = Guid.NewGuid().ToString() },
            //    new BookingStatus { Status = "Completed", Id = Guid.NewGuid().ToString() }
            //    );
        }

        private void ApplyModelsConfigurations(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserConfiguration())
                   .ApplyConfiguration(new SpecializationConfigurations())
                   .ApplyConfiguration(new LawyerConfigurations())
                   .ApplyConfiguration(new ReviewConfiguration())
                   .ApplyConfiguration(new CompanyConfiguration())
                   .ApplyConfiguration(new ServiceConfigurations())
                   .ApplyConfiguration(new BookingStatusConfiguration())
                   .ApplyConfiguration(new BookingConfiguration())
                   .ApplyConfiguration(new RatingConfiguration())
                   .ApplyConfiguration(new LocationConfiguration())
                   .ApplyConfiguration(new UserTypeConfiguration());
        }
        public DbSet<Lawyer> Lawyers { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<BookingStatus> BookingStatuses { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<UserTypeModel> UserTypeModels { get; set; }
    }
}
