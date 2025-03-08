
using Microsoft.EntityFrameworkCore;
using MizanGraduationProject.Data;

namespace MizanGraduationProject.Repositories.Review
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _dbContext;
        public ReviewRepository(AppDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }
        public async Task AddAsync(Data.Models.Review t)
        {
            try
            {
                await _dbContext.Reviews.AddAsync(t);
                await SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteByIdAsync(string id)
        {
            bool isReviewExists = await ExistsAsync(id);
            if (isReviewExists)
            {
                var review = await GetByIdAsync(id);
                _dbContext.Reviews.Remove(review);
                await SaveChangesAsync();
                return !await ExistsAsync(id);
            }
            return false;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _dbContext.Reviews.AnyAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Data.Models.Review>> GetAllAsync()
        {
            return await _dbContext.Reviews.ToListAsync(); 
        }

        public async Task<Data.Models.Review> GetByIdAsync(string id)
        {
            bool isReviewExists = await ExistsAsync(id);
            if (isReviewExists)
            {
                return (await _dbContext.Reviews.Where(e => e.Id == id).FirstOrDefaultAsync())!;
            }
            return null!;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Data.Models.Review t)
        {
            try
            {
                bool isReviewExists = await ExistsAsync(t.Id);
                if (isReviewExists)
                {
                    var existReview = await GetByIdAsync(t.Id);
                    existReview.Comment = t.Comment;
                    existReview.UpdatedAt = DateTime.UtcNow;
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
