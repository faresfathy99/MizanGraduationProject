
using Microsoft.EntityFrameworkCore;
using MizanGraduationProject.Data;

namespace MizanGraduationProject.Repositories.Lawyer
{
    public class LawyerRepository : ILawyerRepository
    {
        private readonly AppDbContext _dbContext;
        public LawyerRepository(AppDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }
        public async Task AddAsync(Data.Models.Lawyer t)
        {
            try
            {
                await _dbContext.Lawyers.AddAsync(t);
                await SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteByIdAsync(string id)
        {
            bool isLawyerExists = await ExistsAsync(id);
            if (isLawyerExists)
            {
                var review = await GetByIdAsync(id);
                _dbContext.Lawyers.Remove(review);
                await SaveChangesAsync();
                return !await ExistsAsync(id);
            }
            return false;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _dbContext.Lawyers.AnyAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Data.Models.Lawyer>> GetAllAsync()
        {
            return await _dbContext.Lawyers.ToListAsync();
        }

        public async Task<Data.Models.Lawyer> GetByIdAsync(string id)
        {
            bool isLawyerExists = await ExistsAsync(id);
            if (isLawyerExists)
            {
                return (await _dbContext.Lawyers.Where(e => e.Id == id).FirstOrDefaultAsync())!;
            }
            return null!;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Data.Models.Lawyer t)
        {
            try
            {
                bool isLawyerExists = await ExistsAsync(t.Id);
                if (isLawyerExists)
                {
                    var existLawyer = await GetByIdAsync(t.Id);
                    existLawyer.Location = t.Location;
                    existLawyer.SpecializationId = t.SpecializationId;
                    existLawyer.UpdatedAt = DateTime.UtcNow;
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
