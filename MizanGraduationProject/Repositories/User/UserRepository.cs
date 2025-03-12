
using Microsoft.EntityFrameworkCore;
using MizanGraduationProject.Data;

namespace MizanGraduationProject.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;
        public UserRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<bool> ExistsByPhone(string phone)
        {
            return await _appDbContext.Users.AnyAsync(e => e.PhoneNumber == phone);
        }
    }
}
