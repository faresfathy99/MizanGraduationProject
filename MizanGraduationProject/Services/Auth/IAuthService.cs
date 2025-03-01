using MizanGraduationProject.Data.DTOs;
using MizanGraduationProject.Data.Models.Identity;
using MizanGraduationProject.Data.Models;
using MizanGraduationProject.Data.Models.ResponseSchema;

namespace MizanGraduationProject.Services.Auth
{
    public interface IAuthService
    {
        Task<ResponseModel<string>> Register(RegisterDTO registerDto);
        Task<ResponseModel<User>> VerifyEmail(string UserId, string Token);
        Task<ResponseModel<LoginResponse>> Login(LoginDTO loginDto);
        Task<ResponseModel<bool>> Logout();
    }
}
