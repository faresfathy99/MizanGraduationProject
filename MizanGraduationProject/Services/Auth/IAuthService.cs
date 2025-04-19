using MizanGraduationProject.Data.DTOs;
using MizanGraduationProject.Data.Models.Identity;
using MizanGraduationProject.Data.Models;
using MizanGraduationProject.Data.Models.ResponseSchema;

namespace MizanGraduationProject.Services.Auth
{
    public interface IAuthService
    {
        Task<ResponseModel<string>> Register(RegisterDTO registerDto);
        Task<ResponseModel<User>> VerifyEmail(VerifyEmailDTO verifyEmailDTO);
        Task<ResponseModel<string>> GetNewVerifyEmailCode(NewVerifyEmailCodeDTO newVerifyEmailCodeDTO);
        Task<ResponseModel<LoginResponse>> Login(LoginDTO loginDto);
        Task<ResponseModel<LoginResponse>> Login2fa(Login2faDTO login2FaDTO);
        Task<ResponseModel<bool>> Logout();
        Task<ResponseModel<string>> ForgetPasswordAsync(ForgetPasswordDto forgetPasswordDto);
        Task<ResponseModel<string>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<ResponseModel<string>> EnableTwoFactorAuthenticationAsync(EnableDisable2fa enableDisable2Fa);
        Task<ResponseModel<string>> DisableTwoFactorAuthenticationAsync(EnableDisable2fa enableDisable2Fa);
    }
}
