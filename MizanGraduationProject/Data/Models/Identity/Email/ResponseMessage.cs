namespace MizanGraduationProject.Data.Models.Identity.Email
{
    public class ResponseMessage
    {
        public static string GetEmailSuccessMessage(string emailAddress)
        {
            return $"Email sent successfully to {emailAddress}";
        }
    }
}
