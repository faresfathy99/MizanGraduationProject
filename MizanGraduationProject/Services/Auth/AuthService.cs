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

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager,
        IConfiguration configuration, IEmailService emailService,
        RoleManager<IdentityRole> roleManager, AppDbContext dbContext, IUserRepository userRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailService = emailService;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _userRepository = userRepository;
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
            var user = _GetUserFromRegisterDto(registerDto);
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var createUser = await _userManager.CreateAsync(user, registerDto.Password);
                await assignRolesToUser(user, null!);
                await transaction.CommitAsync();
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                return new ResponseModel<string>
                {
                    Success = true,
                    Message = "Account created successfully",
                    StatusCode = 201,
                    Model = token
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

        public async Task<ResponseModel<User>> VerifyEmail(string email, string Token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var verify = await _userManager.ConfirmEmailAsync(user, Token);
                if (verify.Succeeded)
                {
                    return new ResponseModel<User>
                    {
                        Success = true,
                        Message = "Email Verified Successfully",
                        StatusCode = 200
                    };
                }
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
                string token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                return new ResponseModel<LoginResponse>
                {
                    Success = true,
                    Message = "2fa",
                    StatusCode = 200,
                    Model = new LoginResponse { AccessToken = new TokenType { Token = token, ExpiryTokenDate = DateTime.Now.AddMinutes(10) } }
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

        //public async Task<ResponseModel<string>> EnableTwoFactorAuthenticationAsync(EnableDisable2fa enableDisable2Fa)
        //{
        //    var user = await _userManager.FindByEmailAsync(enableDisable2Fa.Email);
        //    if (user == null)
        //    {
        //        return new ResponseModel<string>
        //        {
        //            Success = false,
        //            Message = $"user not found",
        //            StatusCode = 404
        //        };
        //    }
        //    if (user.TwoFactorEnabled)
        //    {
        //        return new ResponseModel<string>
        //        {
        //            Success = false,
        //            Message = $"Two factor authentication active",
        //            StatusCode = 403
        //        };
        //    }
        //    await _userManager.SetTwoFactorEnabledAsync(user, true);
        //    await _userManager.UpdateAsync(user);
        //    return new ResponseModel<string>
        //    {
        //        Success = true,
        //        Message = $"Two factor authentication enabled successfully",
        //        StatusCode = 200,
        //    };
        //}

        //public async Task<ResponseModel<string>> DisableTwoFactorAuthenticationAsync(EnableDisable2fa enableDisable2Fa)
        //{
        //    var user = await _userManager.FindByEmailAsync(enableDisable2Fa.Email);
        //    if (user == null)
        //    {
        //        return new ResponseModel<string>
        //        {
        //            Success = false,
        //            Message = $"user not found",
        //            StatusCode = 404
        //        };
        //    }
        //    if (!user.TwoFactorEnabled)
        //    {
        //        return new ResponseModel<string>
        //        {
        //            Success = false,
        //            Message = $"Two factor authentication not active",
        //            StatusCode = 403
        //        };
        //    }
        //    await _userManager.SetTwoFactorEnabledAsync(user, false);
        //    await _userManager.UpdateAsync(user);
        //    return new ResponseModel<string>
        //    {
        //        Success = true,
        //        Message = $"Two factor authentication disabled successfully",
        //        StatusCode = 200,
        //    };
        //}

        public async Task<ResponseModel<LoginResponse>> Login2fa(Login2faDTO login2FaDTO)
        {
            //await _signInManager.PasswordSignInAsync(login2FaDTO.Email, "", false, false);
            var user = await _userManager.FindByEmailAsync(login2FaDTO.Email);
            var signIn = await _signInManager.TwoFactorSignInAsync("Email", login2FaDTO.Token, false, true);
            if (!signIn.Succeeded && user != null)
            {
                return await _GetJwtTokenAsync(user);
            }
            return new ResponseModel<LoginResponse>
            {
                Success = false,
                Message = $"invalid email or token",
                StatusCode = 400
            };
        }
    }
}
