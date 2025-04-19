using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MizanGraduationProject.Data.DTOs;
using MizanGraduationProject.Data.Models.Identity;
using MizanGraduationProject.Data.Models.ResponseSchema;
using MizanGraduationProject.Data.Models;
using MizanGraduationProject.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MizanGraduationProject.Services.Email;
using MizanGraduationProject.Repositories.User;
using MizanGraduationProject.Repositories.VerifyOtp;
using System.Security.Policy;
using MizanGraduationProject.Data.Models.Identity.Email;
using MizanGraduationProject.Data.Models.OTP;
using MizanGraduationProject.Repositories.LoginOTP;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.EntityFrameworkCore;
using MizanGraduationProject.Repositories.Lawyer;

namespace MizanGraduationProject.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _dbContext;
        private readonly IUserRepository _userRepository;
        private readonly IVerifyOTPRepository _verifyOTPRepository;
        private readonly ILoginOTPRepository _loginOTPRepository;
        private readonly AppDbContext _appDbContext;
        private readonly ILawyerRepository _lawyerRepository;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager,
        IConfiguration configuration, IEmailService emailService,
        RoleManager<IdentityRole> roleManager, AppDbContext dbContext, IUserRepository userRepository, 
        IVerifyOTPRepository verifyOTPRepository, ILoginOTPRepository loginOTPRepository, 
        AppDbContext appDbContext, ILawyerRepository lawyerRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailService = emailService;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _userRepository = userRepository;
            _verifyOTPRepository = verifyOTPRepository;
            _loginOTPRepository = loginOTPRepository;
            _appDbContext = appDbContext;
            _lawyerRepository = lawyerRepository;
        }

        private async Task assignRolesToUser(User user, List<string> roles)
        {
            if (roles == null || roles.Count == 0)
            {
                roles = new List<string>() { "USER" };
            }
            foreach (var role in roles)
            {
                if (await _roleManager.RoleExistsAsync(role))
                {
                    if (await _userManager.IsInRoleAsync(user, role)) continue;
                    await _userManager.AddToRoleAsync(user, role);
                }
            }
        }

        public async Task<ResponseModel<string>> Register(RegisterDTO registerDto)
        {
            var existsByEmail = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existsByEmail != null)
            {
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = "Choose another email address",
                    StatusCode = 403
                };
            }
            var existsByPhone = await _userRepository.ExistsByPhone(registerDto.PhoneNumber);
            if (existsByPhone)
            {
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = "Choose another phone number",
                    StatusCode = 403
                };
            }
            var userType = await _appDbContext.UserTypeModels.Where(e => e.NormalizedName == registerDto.UserType.ToUpper()).FirstOrDefaultAsync();
            if(userType == null)
            {
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = "Invalid user Type",
                    StatusCode = 403
                };
            }
            var user = _GetUserFromRegisterDto(registerDto);
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var createUser = await _userManager.CreateAsync(user, registerDto.Password);
                await assignRolesToUser(user, null!);
                if(userType.NormalizedName == "LAWYER")
                {
                    await _lawyerRepository.AddAsync(new Data.Models.Lawyer
                    {
                        UserId = user.Id,
                    });
                }
                string code = _GenerateCode();
                var verify = await _verifyOTPRepository.GetByUserIdAsync(user.Id);
                if(verify != null)
                {
                    await _verifyOTPRepository.DeleteByUserIdAsync(user.Id);
                }
                await _verifyOTPRepository.AddAsync(new Data.Models.OTP.VerifyOTP
                {
                    Code = code,
                    UserID = user.Id
                });
                try
                {
                    _emailService.SendEmail(new Message(new string[] { registerDto.Email },
                            $"Confirmation Email Code", $"{code}"));
                }
                catch (Exception ex) 
                {
                    throw new Exception(ex.Message);
                }
                await transaction.CommitAsync();
                return new ResponseModel<string>
                {
                    Success = true,
                    Message = "Account created successfully, please check your inbox",
                    StatusCode = 201,
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
            }
            return new ResponseModel<string>
            {
                Success = false,
                Message = "failed to register",
                StatusCode = 500,
            };
        }

        public async Task<ResponseModel<User>> VerifyEmail(VerifyEmailDTO verifyEmailDTO)
        {
            var user = await _userManager.FindByEmailAsync(verifyEmailDTO.Email);
            if (user == null)
            {
                return new ResponseModel<User>
                {
                    Success = false,
                    Message = "Failed To Verify Email",
                    StatusCode = 500
                };
            }
            VerifyOTP verify = await _verifyOTPRepository.GetByUserIdAsync(user.Id);
            if (verify == null)
            {
                return new ResponseModel<User>
                {
                    Success = false,
                    Message = "Failed To Verify Email",
                    StatusCode = 500
                };
            }
            if(verify.ExpiredAt < DateTime.UtcNow)
            {
                return new ResponseModel<User>
                {
                    Success = true,
                    Message = "Token Has Expired",
                    StatusCode = 200
                };
            }
            if(verify.Code == verifyEmailDTO.Token)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
                await _verifyOTPRepository.DeleteByIdAsync(verify.ID);
                return new ResponseModel<User>
                {
                    Success = true,
                    Message = "Email Verified Successfully",
                    StatusCode = 200
                };
            }
            return new ResponseModel<User>
            {
                Success = false,
                Message = "Failed To Verify Email",
                StatusCode = 500
            };
        }

        private User _GetUserFromRegisterDto(RegisterDTO registerDto)
        {
            return new User
            {
                UserName = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                PhoneNumber = registerDto.PhoneNumber
            };
        }

        public async Task<ResponseModel<LoginResponse>> Login(LoginDTO loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !user.EmailConfirmed)
            {
                return new ResponseModel<LoginResponse>
                {
                    Success = true,
                    Message = "Failed To Login Please Try Again",
                    StatusCode = 500
                };
            }
            await _signInManager.SignOutAsync();
            var checkLogin = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!checkLogin.Succeeded)
            {
                return new ResponseModel<LoginResponse>
                {
                    Success = false,
                    Message = "Invalid username or password",
                    StatusCode = 406
                };
            }
            if (user.TwoFactorEnabled)
            {
                var verify = await _loginOTPRepository.GetByUserIdAsync(user.Id);
                if(verify != null)
                {
                    await _loginOTPRepository.DeleteByUserIdAsync(user.Id);
                }
                string code = _GenerateCode();
                await _loginOTPRepository.AddAsync(new Data.Models.OTP.LoginOTP
                {
                    Code = code,
                    UserID = user.Id
                });
                try
                {
                    _emailService.SendEmail(new Message(new string[] { user.Email! },
                            $"2fa Code", $"{code}"));
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return new ResponseModel<LoginResponse>
                {
                    Success = true,
                    Message = "Check your inbox to get 2fa code",
                    StatusCode = 200,
                };
                
            }
            return await _GetJwtTokenAsync(user);
        }

        private async Task<JwtSecurityToken> _GenerateUserToken(User user)
        {
            var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            var jwtToken = _GetToken(authClaims);
            return jwtToken;
        }

        private JwtSecurityToken _GetToken(List<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));
            _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);
            var expirationTime = DateTime.UtcNow.AddMinutes(tokenValidityInMinutes);
            var localTimeZone = TimeZoneInfo.Local;
            var expirationTimeInLocalTimeZone = TimeZoneInfo.ConvertTimeFromUtc(expirationTime, localTimeZone);
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: expirationTimeInLocalTimeZone,
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }

        private async Task<ResponseModel<LoginResponse>> _GetJwtTokenAsync(User user)
        {
            var ResponseObject = new LoginResponse
            {
                AccessToken = new TokenType
                {
                    ExpiryTokenDate = (await _GenerateUserToken(user)).ValidTo,
                    Token = new JwtSecurityTokenHandler().WriteToken(
                    await _GenerateUserToken(user))
                }
            };
            return new ResponseModel<LoginResponse>
            {
                Message = "Token Generated Successfully",
                Model = ResponseObject,
                StatusCode = 200,
                Success = true
            };
        }

        public async Task<ResponseModel<bool>> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync().ConfigureAwait(false);
                return new ResponseModel<bool>
                {
                    Success = true,
                    Message = "Logged out successfully",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<bool>
                {
                    Success = false,
                    Message = $"{ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ResponseModel<string>> ForgetPasswordAsync(ForgetPasswordDto forgetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordDto.Email);
            if (user == null)
            {
                return new ResponseModel<string>
                {
                    Success = true,
                    Message = $"check your inbox",
                    StatusCode = 200
                };
            }
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return new ResponseModel<string>
            {
                Success = true,
                Message = $"check your inbox",
                StatusCode = 200,
                Model = token
            };
        }

        public async Task<ResponseModel<string>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = $"failed to upadate password",
                    StatusCode = 500
                };
            }
            var result = await _userManager.ResetPasswordAsync(user,
                resetPasswordDto.Token, resetPasswordDto.Password);
            if (result.Succeeded)
            {
                return new ResponseModel<string>
                {
                    Success = true,
                    Message = $"password updated successfully",
                    StatusCode = 200
                };
            }
            return new ResponseModel<string>
            {
                Success = false,
                Message = $"failed to upadate password",
                StatusCode = 500
            };
        }

        public async Task<ResponseModel<string>> EnableTwoFactorAuthenticationAsync(EnableDisable2fa enableDisable2Fa)
        {
            var user = await _userManager.FindByEmailAsync(enableDisable2Fa.Email);
            if (user == null)
            {
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = $"user not found",
                    StatusCode = 404
                };
            }
            if (user.TwoFactorEnabled)
            {
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = $"Two factor authentication active",
                    StatusCode = 403
                };
            }
            await _userManager.SetTwoFactorEnabledAsync(user, true);
            await _userManager.UpdateAsync(user);
            return new ResponseModel<string>
            {
                Success = true,
                Message = $"Two factor authentication enabled successfully",
                StatusCode = 200,
            };
        }

        public async Task<ResponseModel<string>> DisableTwoFactorAuthenticationAsync(EnableDisable2fa enableDisable2Fa)
        {
            var user = await _userManager.FindByEmailAsync(enableDisable2Fa.Email);
            if (user == null)
            {
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = $"user not found",
                    StatusCode = 404
                };
            }
            if (!user.TwoFactorEnabled)
            {
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = $"Two factor authentication not active",
                    StatusCode = 403
                };
            }
            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.UpdateAsync(user);
            return new ResponseModel<string>
            {
                Success = true,
                Message = $"Two factor authentication disabled successfully",
                StatusCode = 200,
            };
        }

        public async Task<ResponseModel<LoginResponse>> Login2fa(Login2faDTO login2FaDTO)
        {
            var user = await _userManager.FindByEmailAsync(login2FaDTO.Email);
            if (user == null)
            {
                return new ResponseModel<LoginResponse>
                {
                    Success = false,
                    Message = $"invalid email or token",
                    StatusCode = 400
                };
            }
            var verify = await _loginOTPRepository.GetByUserIdAsync(user.Id);
            if(verify == null)
            {
                return new ResponseModel<LoginResponse>
                {
                    Success = false,
                    Message = $"invalid email or token",
                    StatusCode = 400
                };
            }
            if(verify.ExpiredAt < DateTime.UtcNow)
            {
                return new ResponseModel<LoginResponse>
                {
                    Success = false,
                    Message = $"Token Expired",
                    StatusCode = 500
                };
            }
            if(login2FaDTO.Token == verify.Code)
            {
                await _loginOTPRepository.DeleteByUserIdAsync(user.Id);
                return await _GetJwtTokenAsync(user);
            }
            return new ResponseModel<LoginResponse>
            {
                Success = false,
                Message = $"invalid email or token",
                StatusCode = 400
            };
        }

        private string _GenerateCode()
        {
            StringBuilder stringBuilder = new StringBuilder();
            Random random = new Random();
            for (int i=1;i<=6; i++)
            {
                stringBuilder.Append($"{random.Next(0, 9)}");
            }
            return stringBuilder.ToString();
        }

        public async Task<ResponseModel<string>> GetNewVerifyEmailCode(NewVerifyEmailCodeDTO newVerifyEmailCodeDTO)
        {
            var user = await _userManager.FindByEmailAsync(newVerifyEmailCodeDTO.Email);
            if (user == null)
            {
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = "Check your inbox",
                    StatusCode = 200
                };
            }
            if (user.EmailConfirmed)
            {
                return new ResponseModel<string>
                {
                    Success = false,
                    Message = "Email Already Verified",
                    StatusCode = 403
                };
            }
            VerifyOTP verify = await _verifyOTPRepository.GetByUserIdAsync(user.Id);
            if (verify != null)
            {
                await _verifyOTPRepository.DeleteByIdAsync(verify.ID);
            }
            string code = _GenerateCode();
            await _verifyOTPRepository.AddAsync(new Data.Models.OTP.VerifyOTP
            {
                Code = code,
                UserID = user.Id
            });
            try
            {
                _emailService.SendEmail(new Message(new string[] { newVerifyEmailCodeDTO.Email },
                        $"Confirmation Email Code", $"{code}"));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return new ResponseModel<string>
            {
                Success = false,
                Message = "Check your inbox",
                StatusCode = 200
            };
        }
    }
}
