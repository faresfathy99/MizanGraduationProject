namespace MizanGraduationProject.Repositories.User
{
    public interface IUserRepository
    {
        Task<bool> ExistsByPhone(string phone);
    }
}
