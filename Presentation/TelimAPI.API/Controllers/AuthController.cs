using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TelimAPI.API.Common;
using TelimAPI.API.Common.Helper;
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
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponses.Fail<object>(
                    "Əməliyyat uğursuz oldu",
                    result.Errors
                ));
            }
            return Ok(ApiResponses.Success<object>(
                message: "Qeydiyyat uğurla tamamlandı. İndi daxil ola bilərsiniz."
            ));
        }

        [HttpPost("RegisterAdmin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterUserAsync(dto, "Admin");

            if (!result.Succeeded)
            {
                return BadRequest(ApiResponses.Fail<object>(
                    "Əməliyyat uğursuz oldu",
                    result.Errors
                ));
            }
            return Ok(ApiResponses.Success<object>(
                message: "Qeydiyyat uğurla tamamlandı. İndi daxil ola bilərsiniz."
            ));
        }

        [HttpPost("RegisterTrainer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterTrainer([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterUserAsync(dto, "Trainer");
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponses.Fail<object>(
                    "Əməliyyat uğursuz oldu",
                    result.Errors
                ));
            }
            return Ok(ApiResponses.Success<object>(
                message: "Qeydiyyat uğurla tamamlandı. İndi daxil ola bilərsiniz."
            ));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginUserAsync(dto);

            if (!result.Succeeded)
            {
                return Unauthorized(ApiResponses.Fail<object>(
                    "Daxilolma uğursuz oldu",
                    result.Errors
                ));
            }
            return Ok(ApiResponses.Success(
                new LoginResponseDto
                {
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken
                },
                "Daxilolma uğurlu oldu"
    ));
        }


        [AllowAnonymous]
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
        {
            
            var resetPasswordApiUrl = Url.Action(nameof(ResetPassword), "Auth", null, Request.Scheme);

            
            var result = await _authService.ForgotPasswordAsync(dto, resetPasswordApiUrl);


            if (!result.Succeeded)
            {
                return BadRequest(ApiResponses.Fail<object>(
                    "Şifrə bərpası uğursuz oldu",
                    result.Errors
                ));
            }

            return Ok(ApiResponses.Success<object>(
                message: "Əgər istifadəçi mövcuddursa, şifrə bərpa linki e-poçta göndərildi."
            ));
        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string token, [FromBody] ResetPasswordDto dto)
        {
            
            if (!string.IsNullOrEmpty(token))
            {
                token = System.Net.WebUtility.UrlDecode(token);
            }

            var result = await _authService.ResetPasswordAsync(dto, token);

            if (!result.Succeeded)
            {
                return BadRequest(ApiResponses.Fail<object>(
                    message: "Şifrə bərpası uğursuz oldu. Token etibarsız ola bilər və ya şifrə tələblərə cavab vermir.",
                    errors: result.Errors 
                ));
            }

            return Ok(ApiResponses.Success<object>(
            message: "Şifrəniz uğurla bərpa edildi."
            ));
        }


        [HttpGet("GetCurrentUser")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            if (User?.Identity?.IsAuthenticated != true)
            {
                return Unauthorized(ApiResponses.Fail<object>("İstifadəçi daxil olmayıb"));
            }

            return Ok(ApiResponses.Success(
                new CurrentUserDto
                {
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Email = User.FindFirstValue(ClaimTypes.Email),
                    Role = User.FindFirstValue(ClaimTypes.Role)
                },
                "Cari istifadəçi məlumatları alındı"
            ));
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            
            var result = await _authService.RefreshTokenAsync(request.RefreshToken);

            if (!result.Succeeded)
            {
                return Unauthorized(ApiResponses.Fail<object>(
                    "Token yenilənmədi",
                    result.Errors
                ));
            }


            return Ok(ApiResponses.Success(
                new LoginResponseDto
                {
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken
                },
                "Tokenlər yeniləndi"
            ));
        }

        [Authorize]
        [HttpPost("RevokeRefreshToken")]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenRequest request)
        {
           
            var success = await _authService.RevokeRefreshTokenAsync(request.RefreshToken);

            if (!success)
            {
                return BadRequest(ApiResponses.Fail<object>(
                    message: "Çıxış zamanı xəta baş verdi."
                ));
            }


            return Ok(ApiResponses.Success<object>(
                message: "Çıxış uğurlu oldu. Refresh Token ləğv edildi."
            ));
        }
    }
}
