namespace MizanGraduationProject.Repositories.Crud
{
    public interface ICrud <T>
    {
        Task AddAsync(T t);
        Task<bool> UpdateAsync(T t);
        Task<bool> DeleteByIdAsync(string id);
        Task<T> GetByIdAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task SaveChangesAsync();
        Task<IEnumerable<T>> GetAllAsync();
    }
}
