
using Microsoft.EntityFrameworkCore;
using MizanGraduationProject.Data;

namespace MizanGraduationProject.Repositories.BookingStatus
{
    public class BookingStatusRepository : IBookingStatusRepository
    {
        private readonly AppDbContext _dbContext;
        public BookingStatusRepository(AppDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }
        public async Task AddAsync(Data.Models.BookingStatus t)
        {
            try
            {
                await _dbContext.BookingStatuses.AddAsync(t);
                await SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteByIdAsync(string id)
        {
            bool isBookingStatusesExists = await ExistsAsync(id);
            if (isBookingStatusesExists)
            {
                var bookingStatuses = await GetByIdAsync(id);
                _dbContext.BookingStatuses.Remove(bookingStatuses);
                await SaveChangesAsync();
                return !await ExistsAsync(id);
            }
            return false;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _dbContext.BookingStatuses.AnyAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Data.Models.BookingStatus>> GetAllAsync()
        {
            return await _dbContext.BookingStatuses.ToListAsync();
        }

        public async Task<Data.Models.BookingStatus> GetByIdAsync(string id)
        {
            bool isBookingStatusesExists = await ExistsAsync(id);
            if (isBookingStatusesExists)
            {
                return (await _dbContext.BookingStatuses.Where(e => e.Id == id).FirstOrDefaultAsync())!;
            }
            return null!;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Data.Models.BookingStatus t)
        {
            try
            {
                bool isBookingStatusesExists = await ExistsAsync(t.Id);
                if (isBookingStatusesExists)
                {
                    var existBookingStatus = await GetByIdAsync(t.Id);
                    existBookingStatus.Status = t.Status;
                    existBookingStatus.UpdatedAt = DateTime.UtcNow;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
