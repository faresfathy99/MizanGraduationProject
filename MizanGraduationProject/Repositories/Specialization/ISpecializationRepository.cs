using MizanGraduationProject.Repositories.Crud;

namespace MizanGraduationProject.Repositories.Specialization
{
    public interface ISpecializationRepository : ICrud<Data.Models.Specialization>
    {
        public Task<Data.Models.Specialization> GetByNameAsync(string name);
        public Task<bool> ExistsBynameAsync(string name);
    }
}
