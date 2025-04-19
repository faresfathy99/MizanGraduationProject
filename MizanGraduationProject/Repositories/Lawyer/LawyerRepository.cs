
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MizanGraduationProject.Data;
using MizanGraduationProject.Data.Classes.Filter;
using MizanGraduationProject.Data.DTOs;
using MizanGraduationProject.Data.Models;
using MizanGraduationProject.Repositories.Specialization;

namespace MizanGraduationProject.Repositories.Lawyer
{
    public class LawyerRepository : ILawyerRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ISpecializationRepository _specializationRepository;
        private readonly UserManager<Data.Models.User> _userManager;
        public LawyerRepository(AppDbContext _dbContext, ISpecializationRepository specializationRepository,
            UserManager<Data.Models.User> userManager)
        {
            this._dbContext = _dbContext;
            _specializationRepository = specializationRepository;
            _userManager = userManager;
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

        public async Task<IEnumerable<object>> GetAllWithFilterAsync(FilterLawyersDTO filterLawyersDTO)
        {
            var specialization = await _specializationRepository.GetByNameAsync(filterLawyersDTO.Specialization);
            var location = await _dbContext.Locations.Where(e=>e.NormalizedName==filterLawyersDTO.Specialization.ToUpper()).FirstOrDefaultAsync();
            if (specialization != null && location == null)
            {
                var lawyers = await _dbContext.Lawyers
                    .Where(
                    e=>e.SpecializationId == specialization.Id)
                    .ToListAsync();
                return await _Result(lawyers);
            }
            if (specialization == null && location != null)
            {
                var lawyers = await _dbContext.Lawyers
                    .Where(
                    e => e.Location.ToUpper() == location.NormalizedName)
                    .ToListAsync();
                return await _Result(lawyers);
            }
            if (specialization != null && location != null)
            {
                var lawyers = await _dbContext.Lawyers
                    .Where(
                    e => e.Location.ToUpper() == location.NormalizedName)
                    .Where(
                    e=>e.SpecializationId == specialization.Id
                    )
                    .ToListAsync();

                return await _Result(lawyers);
            }
            var result = (await GetAllAsync()).ToList();
            return await _Result(result);
        }

        private async Task<IEnumerable<FilterResult>> _Result(List<Data.Models.Lawyer> lawyers)
        {
            List<FilterResult> results = new List<FilterResult>();
            foreach(var lawyer in lawyers)
            {
                var user = await _userManager.FindByIdAsync(lawyer.UserId);
                if (user == null) continue;
                var specialization = await _specializationRepository.GetByIdAsync(lawyer.SpecializationId);
                results.Add(new FilterResult
                {
                    Id = user.Id,
                    Location = lawyer.Location,
                    Name = $"{user.FirstName} {user.LastName}",
                    Specialization = specialization == null ? null! : specialization.NormalizedName
                });
            }
            return results;
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
                    await _dbContext.SaveChangesAsync();
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
