
using Microsoft.EntityFrameworkCore;
using MizanGraduationProject.Data;

namespace MizanGraduationProject.Repositories.Service
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly AppDbContext _dbContext;
        public ServiceRepository(AppDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }
        public async Task AddAsync(Data.Models.Service t)
        {
            try
            {
                await _dbContext.Services.AddAsync(t);
                await SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteByIdAsync(string id)
        {
            bool isServiceExists = await ExistsAsync(id);
            if (isServiceExists)
            {
                var service = await GetByIdAsync(id);
                _dbContext.Services.Remove(service);
                await SaveChangesAsync();
                return !await ExistsAsync(id);
            }
            return false;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _dbContext.Ratings.AnyAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Data.Models.Service>> GetAllAsync()
        {
            return await _dbContext.Services.ToListAsync();
        }

        public async Task<Data.Models.Service> GetByIdAsync(string id)
        {
            bool isServiceExists = await ExistsAsync(id);
            if (isServiceExists)
            {
                return (await _dbContext.Services.Where(e => e.Id == id).FirstOrDefaultAsync())!;
            }
            return null!;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Data.Models.Service t)
        {
            try
            {
                bool isServiceExists = await ExistsAsync(t.Id);
                if (isServiceExists)
                {
                    var existService = await GetByIdAsync(t.Id);
                    existService.ServiceName = t.ServiceName;
                    existService.Price = t.Price;
                    existService.Description = t.Description;
                    existService.UpdatedAt = DateTime.UtcNow;
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
