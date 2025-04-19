using MizanGraduationProject.Data.Classes.Filter;
using MizanGraduationProject.Data.DTOs;
using MizanGraduationProject.Repositories.Crud;

namespace MizanGraduationProject.Repositories.Lawyer
{
    public interface ILawyerRepository : ICrud<Data.Models.Lawyer>
    {
        Task<IEnumerable<object>> GetAllWithFilterAsync(FilterLawyersDTO filterLawyersDTO);
    }
}
