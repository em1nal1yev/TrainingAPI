using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Application.Common.Results;
using TelimAPI.Application.DTOs.Auth;

namespace TelimAPI.Application.Services
{
    public interface IAuthService
    {
        Task<Result> RegisterUserAsync(RegisterDto model, string roleName);
        Task<Result<LoginResponseDto>> LoginUserAsync(LoginDto model);
        Task<Result<LoginResponseDto>> RefreshTokenAsync(string refreshToken);
        Task<Result> RevokeRefreshTokenAsync(string refreshToken);
        Task<Result> ForgotPasswordAsync(ForgotPasswordRequestDto dto, string resetPasswordApiUrl);
        Task<Result> ResetPasswordAsync(ResetPasswordDto dto, string token);
    }
}
