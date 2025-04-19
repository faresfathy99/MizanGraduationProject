using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MizanGraduationProject.Data.DTOs;
using MizanGraduationProject.Data.Models.Identity.Email;
using MizanGraduationProject.Data.Models.ResponseSchema;
using MizanGraduationProject.Services.Auth;
using MizanGraduationProject.Services.Email;
using System.Security.Policy;

namespace MizanGraduationProject.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        public AuthController(IAuthService authService, IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
        }
        [HttpPost("register")]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            try
            {
                var response = await _authService.Register(registerDto);
                if (response.Success)
                {
                    return StatusCode(201, response);
                }
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    Success = false,
                    StatusCode = 500
                });
            }
        }

        [HttpPost("verify-email")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDTO verifyEmailDTO)
        {
            try
            {
                var response = await _authService.VerifyEmail(verifyEmailDTO);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<bool>
                {
                    Message = ex.Message,
                    Success = false,
                    StatusCode = 500
                });
            }
        }

        [HttpPost("get-new-verify-email-code")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetNewVerifyEmailCode([FromBody] NewVerifyEmailCodeDTO newVerifyEmailCodeDTO)
        {
            try
            {
                var response = await _authService.GetNewVerifyEmailCode(newVerifyEmailCodeDTO);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel<bool>
                {
                    Message = ex.Message,
                    Success = false,
                    StatusCode = 500
                });
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        [ProducesResponseType(406)]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                var response = await _authService.Login(loginDto);
                if (response.Success)
                {
                    return Ok(response);
                }
                return StatusCode(406, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    Success = false,
                    StatusCode = 500
                });
            }
        }

        [HttpPost("login-2fa")]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Login2fa([FromBody] Login2faDTO login2FaDTO)
        {
            try
            {
                var response = await _authService.Login2fa(login2FaDTO);
                if (response.Success)
                {
                    return Ok(response);
                }
                return StatusCode(400, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    Success = false,
                    StatusCode = 500
                });
            }
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            var logout = await _authService.Logout();
            if (logout.Success) return Ok(logout);
            return StatusCode(500, logout);
        }

        [HttpPost("forget-password")]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        [ProducesResponseType(406)]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto forgetPasswordDto)
        {
            try
            {
                var response = await _authService.ForgetPasswordAsync(forgetPasswordDto);
                if (response.Model != null)
                {
                    var message = new Message(new string[] { forgetPasswordDto.Email },
                            "Reset password Token", response.Model);
                    _emailService.SendEmail(message);
                    return Ok(new
                    {
                        Success = response.Success,
                        StatusCode = response.StatusCode,
                        Message = response.Message
                    });
                }
                return StatusCode(400, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    Success = false,
                    StatusCode = 500
                });
            }
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        [ProducesResponseType(406)]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var response = await _authService.ResetPasswordAsync(resetPasswordDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    Success = false,
                    StatusCode = 500
                });
            }
        }

        [HttpPost("enable-2fa")]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        [ProducesResponseType(406)]
        public async Task<IActionResult> EnableTwoFactorAuthenticationAsync([FromBody] EnableDisable2fa enableDisable2Fa)
        {
            try
            {
                var response = await _authService.EnableTwoFactorAuthenticationAsync(enableDisable2Fa);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    Success = false,
                    StatusCode = 500
                });
            }
        }

        [HttpPost("disable-2fa")]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        [ProducesResponseType(406)]
        public async Task<IActionResult> DisableTwoFactorAuthenticationAsync([FromBody] EnableDisable2fa enableDisable2Fa)
        {
            try
            {
                var response = await _authService.DisableTwoFactorAuthenticationAsync(enableDisable2Fa);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    Success = false,
                    StatusCode = 500
                });
            }
        }


    }
}
