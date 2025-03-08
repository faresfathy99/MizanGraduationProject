
using Microsoft.EntityFrameworkCore;
using MizanGraduationProject.Data;

namespace MizanGraduationProject.Repositories.Rating
{
    public class RatingRepository : IRatingRepository
    {
        private readonly AppDbContext _dbContext;
        public RatingRepository(AppDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }
        public async Task AddAsync(Data.Models.Rating t)
        {
            try
            {
                await _dbContext.Ratings.AddAsync(t);
                await SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteByIdAsync(string id)
        {
            bool isRatingExists = await ExistsAsync(id);
            if (isRatingExists)
            {
                var rating = await GetByIdAsync(id);
                _dbContext.Ratings.Remove(rating);
                await SaveChangesAsync();
                return !await ExistsAsync(id);
            }
            return false;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _dbContext.Ratings.AnyAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Data.Models.Rating>> GetAllAsync()
        {
            return await _dbContext.Ratings.ToListAsync();
        }

        public async Task<Data.Models.Rating> GetByIdAsync(string id)
        {
            bool isRatingExists = await ExistsAsync(id);
            if (isRatingExists)
            {
                return (await _dbContext.Ratings.Where(e => e.Id == id).FirstOrDefaultAsync())!;
            }
            return null!;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Data.Models.Rating t)
        {
            try
            {
                bool isRatingExists = await ExistsAsync(t.Id);
                if (isRatingExists)
                {
                    var existRating = await GetByIdAsync(t.Id);
                    existRating.Rate = t.Rate;
                    existRating.UpdatedAt = DateTime.UtcNow;
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
