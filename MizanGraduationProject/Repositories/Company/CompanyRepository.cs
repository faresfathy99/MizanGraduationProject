
using Microsoft.EntityFrameworkCore;
using MizanGraduationProject.Data;

namespace MizanGraduationProject.Repositories.Company
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly AppDbContext _dbContext;
        public CompanyRepository(AppDbContext _dbContext)
        {
            this._dbContext = _dbContext;
        }

        public async Task AddAsync(Data.Models.Company t)
        {
            try
            {
                await _dbContext.Companies.AddAsync(t);
                await SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteByIdAsync(string id)
        {
            bool isCompanyExists = await ExistsAsync(id);
            if (isCompanyExists)
            {
                var company = await GetByIdAsync(id);
                _dbContext.Companies.Remove(company);
                await SaveChangesAsync();
                return !await ExistsAsync(id);
            }
            return false;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _dbContext.Companies.AnyAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Data.Models.Company>> GetAllAsync()
        {
            return await _dbContext.Companies.ToListAsync();
        }

        public async Task<Data.Models.Company> GetByIdAsync(string id)
        {
            bool isCompanyExists = await ExistsAsync(id);
            if (isCompanyExists)
            {
                return (await _dbContext.Companies.Where(e => e.Id == id).FirstOrDefaultAsync())!;
            }
            return null!;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Data.Models.Company t)
        {
            try
            {
                bool isCompanyExists = await ExistsAsync(t.Id);
                if (isCompanyExists)
                {
                    var existCompany = await GetByIdAsync(t.Id);
                    existCompany.Address = t.Address;
                    existCompany.Email = t.Email;
                    existCompany.Name = t.Name;
                    existCompany.Phone = t.Phone;
                    existCompany.UpdatedAt = DateTime.UtcNow;
                    await SaveChangesAsync();
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
