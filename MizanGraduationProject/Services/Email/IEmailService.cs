using MizanGraduationProject.Data.Models.Identity.Email;

namespace MizanGraduationProject.Services.Email
{
    public interface IEmailService
    {
        string SendEmail(Message message);
    }
}
