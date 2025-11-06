using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
                return Ok(new { Message = "Admin istifadəçisi uğurla yaradıldı." });
            }
            return BadRequest(new { Errors = result.Errors });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginUserAsync(dto);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Daxilolma uğurlu oldu." });
            }
            return Unauthorized(new { Errors = result.Errors });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> logout()
        {
            await _authService.SignOutUserAsync();
            return Ok(new { Message = "Çıxış uğurlu oldu." });
        }

        [HttpGet("admin-test")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminTest()
        {
            return Ok("Bu səhifəni yalnız Admin rolu görə bilər.");
        }
    }
}
