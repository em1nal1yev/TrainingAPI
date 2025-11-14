using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TelimAPI.Application.DTOs.Auth;
using TelimAPI.Application.Services;

namespace TelimAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterUserAsync(dto, "User");
            if (result.Succeeded)
            {
                return Ok(new { Message = "Qeydiyyat ugurlu oldu. Indi daxil ola bilərsiniz." });
            }
            return BadRequest(new { Error = result.Errors });
        }

        [HttpPost("register-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterUserAsync(dto, "Admin");

            if (result.Succeeded)
            {
                return Ok(new { Message = "Admin hesabi uğurla yaradıldı." });
            }
            return BadRequest(new { Errors = result.Errors });
        }

        [HttpPost("register-trainer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterTrainer([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterUserAsync(dto, "Trainer");
            if (result.Succeeded)
            {
                return Ok(new {Message = "Trainer hesabi ugurla yaradildi"} );
            }
            return BadRequest(new { Errors = result.Errors });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginUserAsync(dto);

            if (result.Succeeded)
            {

                return Ok(new
                {
                    AccessToken = result.AccessToken, 
                    RefreshToken = result.RefreshToken, 
                    Message = "Daxilolma uğurlu oldu."
                });
            }
            return Unauthorized(new { Errors = result.Errors });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var name = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userId == null)
            {
                
                return NotFound(new { Message = "İstifadəçi ID tapılmadı." });
            }

            return Ok(new
            {
                UserId = userId,
                Email = email,
                FullName = name,
                Role = role,
                Message = "Cari istifadəçi məlumatları uğurla alındı."
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            
            var result = await _authService.RefreshTokenAsync(request.RefreshToken);

            if (result.Succeeded)
            {
                
                return Ok(new
                {
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken,
                    Message = "Tokenlər uğurla yeniləndi."
                });
            }

            
            return Unauthorized(new { Errors = result.Errors });
        }

        [Authorize]
        [HttpPost("signout")]
        public async Task<IActionResult> SignOut([FromBody] RefreshTokenRequest request)
        {
           
            var success = await _authService.RevokeRefreshTokenAsync(request.RefreshToken);

            if (success)
            {
                
                return Ok(new { Message = "Çıxış uğurlu oldu. Refresh Token ləğv edildi." });
            }

            
            return BadRequest(new { Message = "Çıxış zamanı xəta baş verdi." });
        }
    }
}
