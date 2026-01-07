using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Application.Common.Results;
using TelimAPI.Application.DTOs.Auth;
using TelimAPI.Application.Repositories;
using TelimAPI.Application.Services;
using TelimAPI.Domain.Entities;

namespace TelimAPI.Persistence.Services
{

    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IEmailService _emailService;
        private readonly ICourtRepository _courtRepository;
        private readonly IDepartmentRepository _departmentRepository;
        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole<Guid>> roleManager, ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository, IEmailService emailService, ICourtRepository courtRepository, IDepartmentRepository departmentRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _emailService = emailService;
            _courtRepository = courtRepository;
            _departmentRepository = departmentRepository;
        }


        public async Task<Result> RegisterUserAsync(RegisterDto dto, string roleName)
        {
            var court = await _courtRepository.GetByIdAsync(dto.CourtId);
            if (court == null) return Result.Failure("Məhkəmə (court) tapılmadı.");

            var department = await _departmentRepository.GetByIdAsync(dto.DepartmentId);
            if (department == null) return Result.Failure("Şöbə (department) tapılmadı.");

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null) return Result.Failure("Bu e-poçt ünvanı artıq qeydiyyatdan keçib.");

            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                Name = dto.Name,
                Surname = dto.Surname,
                EmailConfirmed = true,
                CourtId = dto.CourtId,
                DepartmentId = dto.DepartmentId
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return Result.Failure(result.Errors.Select(e => e.Description).ToList());

            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));

            await _userManager.AddToRoleAsync(user, roleName);

            return Result.Success();
        }

        public async Task<Result<LoginResponseDto>> LoginUserAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Result<LoginResponseDto>.Failure("Yanlış e-poçt və ya şifrə.");

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.CreateAccessToken(user, roles);
            var refreshTokenEntity = _tokenService.CreateRefreshToken(user.Id);

            await _refreshTokenRepository.AddAsync(refreshTokenEntity);

            return Result<LoginResponseDto>.Success(new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenEntity.Token
            });
        }

        public async Task<Result<LoginResponseDto>> RefreshTokenAsync(string refreshToken)
        {
            var existingRefreshToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (existingRefreshToken == null || existingRefreshToken.IsRevoked || existingRefreshToken.Expires < DateTime.UtcNow)
                return Result<LoginResponseDto>.Failure("Etibarsız və ya vaxtı bitmiş Refresh Token.");

            var user = existingRefreshToken.User;
            if (user == null) return Result<LoginResponseDto>.Failure("İstifadəçi tapılmadı.");

            existingRefreshToken.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsync(existingRefreshToken);

            var roles = await _userManager.GetRolesAsync(user);
            var newAccessToken = _tokenService.CreateAccessToken(user, roles);
            var newRefreshTokenEntity = _tokenService.CreateRefreshToken(user.Id);

            await _refreshTokenRepository.AddAsync(newRefreshTokenEntity);

            return Result<LoginResponseDto>.Success(new LoginResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenEntity.Token
            });
        }

        public async Task<Result> RevokeRefreshTokenAsync(string refreshToken)
        {
            var existingRefreshToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (existingRefreshToken != null && !existingRefreshToken.IsRevoked)
            {
                existingRefreshToken.IsRevoked = true;
                await _refreshTokenRepository.UpdateAsync(existingRefreshToken);
            }
            return Result.Success();
        }

        public async Task<Result> ForgotPasswordAsync(ForgotPasswordRequestDto dto, string resetPasswordApiUrl)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null) return Result.Success(); // Təhlükəsizlik üçün uğurlu qaytarırıq

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = System.Web.HttpUtility.UrlEncode(token);
            var resetUrl = $"{resetPasswordApiUrl}?Token={encodedToken}";

            try
            {
                await _emailService.SendAsync(user.Email, "Şifrə Bərpası", $"Link: {resetUrl}");
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Email göndərilmədi: {ex.Message}");
            }
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordDto dto, string token)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null) return Result.Failure("İstifadəçi tapılmadı.");

            var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);
            return result.Succeeded
                ? Result.Success()
                : Result.Failure(result.Errors.Select(e => e.Description).ToList());
        }
    }
}
