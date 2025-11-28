using Microsoft.AspNetCore.Authorization;
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

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterUserAsync(dto, "User");
            if (result.Succeeded)
            {
                return Ok(new { Message = "Qeydiyyat ugurlu oldu. Indi daxil ola bilərsiniz." });
            }
            return BadRequest(new { Error = result.Errors });
        }

        [HttpPost("RegisterAdmin")]
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

        [HttpPost("RegisterTrainer")]
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

        [HttpPost("Login")]
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


        [HttpGet("GetCurrentUser")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return Unauthorized(new { Message = "İstifadəçi daxil olmayıb." });
            }

            
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var emailClaim = User.FindFirst(ClaimTypes.Email);
            var roleClaim = User.FindFirst(ClaimTypes.Role);

            if (userIdClaim == null)
            {
                return NotFound(new { Message = "İstifadəçi ID tapılmadı." });
            }

            return Ok(new
            {
                UserId = userIdClaim?.Value,
                Email = emailClaim?.Value,
                Role = roleClaim?.Value,
                Message = "Cari istifadəçi məlumatları uğurla alındı."
            });
        }

        [Authorize]
        [HttpGet("claims")]
        public IActionResult GetUserClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);                          
        }

        [HttpPost("RefreshToken")]
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
        [HttpPost("RevokeRefreshToken")]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest request)
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
