using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TelimAPI.API.Common;
using TelimAPI.API.Common.Helper;
using TelimAPI.Application.DTOs.Auth;
using TelimAPI.Application.Services;

namespace TelimAPI.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            return await ExecuteAsync(() => _authService.RegisterUserAsync(dto, "User"));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterAdminTrainer([FromBody] RegisterDto dto, string role)
        {
            return await ExecuteAsync(() => _authService.RegisterUserAsync(dto, role));
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            return await ExecuteAsync(() => _authService.LoginUserAsync(dto));
        }


        
        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
        {
            
           return await ExecuteAsync(() => _authService.ForgotPasswordAsync(dto, Url.Action(nameof(ResetPassword), "Auth", null, Request.Scheme)));
        }

        
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string token, [FromBody] ResetPasswordDto dto)
        {
            var decodedToken = !string.IsNullOrEmpty(token) ? System.Net.WebUtility.UrlDecode(token) : token;
            return await ExecuteAsync(() => _authService.ResetPasswordAsync(dto, decodedToken));
        }


        [HttpPost]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            
            return await ExecuteAsync(() => _authService.RefreshTokenAsync(request.RefreshToken));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest request)
        {
           
            return await ExecuteAsync(() => _authService.RevokeRefreshTokenAsync(request.RefreshToken));
        }
    }
}
