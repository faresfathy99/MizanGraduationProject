using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MizanGraduationProject.Data.DTOs;
using MizanGraduationProject.Data.Models.Identity.Email;
using MizanGraduationProject.Data.Models.ResponseSchema;
using MizanGraduationProject.Services.Auth;
using MizanGraduationProject.Services.Email;

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
        public async Task<IActionResult> Register([FromForm] RegisterDTO registerDto)
        {
            try
            {
                var response = await _authService.Register(registerDto);
                if (response.Success)
                {
                    string url = Url.Action(
                        "VerifyEmail",
                        "Auth",
                        new { email = registerDto.Email, Token = response.Model },
                        protocol: HttpContext.Request.Scheme)!;
                    var message = new Message(new string[] { registerDto.Email },
                            "Confirmation Email Token", url);
                    _emailService.SendEmail(message);
                    return Ok(new
                    {
                        Success = response.Success,
                        StatusCode = response.StatusCode,
                        Message = "We sent comfirmation link to your email Check your inbox to verify your email"
                    });
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

        [HttpGet("verify-email")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> VerifyEmail(string email, string Token)
        {
            try
            {
                var response = await _authService.VerifyEmail(email, Token);
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
        public async Task<IActionResult> Login([FromForm] LoginDTO loginDto)
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

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            var logout = await _authService.Logout();
            if (logout.Success) return Ok(logout);
            return StatusCode(500, logout);
        }
    }
}
