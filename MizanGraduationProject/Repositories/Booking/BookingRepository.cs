
using Microsoft.EntityFrameworkCore;
using MizanGraduationProject.Data;

namespace MizanGraduationProject.Repositories.Booking
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _dbContext;
        public BookingRepository(AppDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }

        public async Task AddAsync(Data.Models.Booking t)
        {
            try
            {
                await _dbContext.Bookings.AddAsync(t);
                await SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteByIdAsync(string id)
        {
            bool isBookingExists = await ExistsAsync(id);
            if (isBookingExists)
            {
                var booking = await GetByIdAsync(id);
                _dbContext.Bookings.Remove(booking);
                await SaveChangesAsync();
                return !await ExistsAsync(id);
            }
            return false;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _dbContext.Bookings.AnyAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Data.Models.Booking>> GetAllAsync()
        {
            return await _dbContext.Bookings.ToListAsync();
        }

        public async Task<Data.Models.Booking> GetByIdAsync(string id)
        {
            bool isBookingExists = await ExistsAsync(id);
            if (isBookingExists)
            {
                return (await _dbContext.Bookings.Where(e=>e.Id == id).FirstOrDefaultAsync())!;
            }
            return null!;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Data.Models.Booking t)
        {
            try
            {
                bool isBookingExists = await ExistsAsync(t.Id);
                if (isBookingExists)
                {
                    var existBooking = await GetByIdAsync(t.Id);
                    existBooking.BookingDate = t.BookingDate;
                    existBooking.BookingStatus = t.BookingStatus;
                    existBooking.ServiceId = t.ServiceId;
                    existBooking.LawyerId = t.LawyerId;
                    existBooking.UpdatedAt = DateTime.UtcNow;
                    await SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }
    }
}
