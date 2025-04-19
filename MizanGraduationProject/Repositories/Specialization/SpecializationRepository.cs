
using Microsoft.EntityFrameworkCore;
using MizanGraduationProject.Data;

namespace MizanGraduationProject.Repositories.Specialization
{
    public class SpecializationRepository : ISpecializationRepository
    {
        private readonly AppDbContext _dbContext;
        public SpecializationRepository(AppDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }
        public async Task AddAsync(Data.Models.Specialization t)
        {
            try
            {
                await _dbContext.Specializations.AddAsync(t);
                await SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteByIdAsync(string id)
        {
            bool isSpecializationExists = await ExistsAsync(id);
            if (isSpecializationExists)
            {
                var booking = await GetByIdAsync(id);
                _dbContext.Specializations.Remove(booking);
                await SaveChangesAsync();
                return !await ExistsAsync(id);
            }
            return false;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _dbContext.Specializations.AnyAsync(t => t.Id == id);
        }

        public async Task<bool> ExistsBynameAsync(string name)
        {
            return await _dbContext.Specializations.AnyAsync(t => t.NormalizedName == name.ToUpper());
        }

        public async Task<IEnumerable<Data.Models.Specialization>> GetAllAsync()
        {
            return await _dbContext.Specializations.ToListAsync();
        }

        public async Task<Data.Models.Specialization> GetByIdAsync(string id)
        {
            bool isSpecializationExists = await ExistsAsync(id);
            if (isSpecializationExists)
            {
                return (await _dbContext.Specializations.Where(e => e.Id == id).FirstOrDefaultAsync())!;
            }
            return null!;
        }

        public async Task<Data.Models.Specialization> GetByNameAsync(string name)
        {
            bool isSpecializationExists = await ExistsBynameAsync(name);
            if (isSpecializationExists)
            {
                return (await _dbContext.Specializations.Where(e => e.NormalizedName == name.ToUpper()).FirstOrDefaultAsync())!;
            }
            return null!;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Data.Models.Specialization t)
        {
            try
            {
                bool isSpecializationExists = await ExistsAsync(t.Id);
                if (isSpecializationExists)
                {
                    var existSpecialization = await GetByIdAsync(t.Id);
                    existSpecialization.UpdatedAt = DateTime.UtcNow;
                    existSpecialization.Name = t.Name;
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
