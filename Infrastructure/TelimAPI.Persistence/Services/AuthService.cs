using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Application.DTOs.Auth;
using TelimAPI.Application.Repositories;
using TelimAPI.Application.Services;
using TelimAPI.Domain.Entities;

namespace TelimAPI.Persistence.Services
{
    internal class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole<Guid>> roleManager, ITokenService tokenService, IRefreshTokenRepository refreshTokenRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
        }


        public async Task<AuthResult> RegisterUserAsync(RegisterDto dto, string roleName)
        {

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Errors = new[] { "Bu e-poçt ünvanı artıq qeydiyyatdan keçib." }
                };
            }

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

            if (result.Succeeded)
            {
                
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                }

                
                await _userManager.AddToRoleAsync(user, roleName);
            }

            return new AuthResult
            {
                Succeeded = result.Succeeded,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<AuthResult> LoginUserAsync(LoginDto dto)
        {

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Errors = new[] { "Yanlış e-poçt və ya şifrə." }
                };
            }

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.CreateAccessToken(user, roles);
            var refreshTokenEntity = _tokenService.CreateRefreshToken(user.Id);
            await _refreshTokenRepository.AddAsync(refreshTokenEntity);
            return new AuthResult
            {
                Succeeded = true,
                AccessToken = accessToken,
                RefreshToken = refreshTokenEntity.Token,
                Errors = null
            };
        }

        public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
        {
            var existingRefreshToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            
            if (existingRefreshToken == null || existingRefreshToken.IsRevoked || existingRefreshToken.Expires < DateTime.UtcNow)
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Errors = new[] { "Etibarsız və ya vaxtı bitmiş Refresh Token." }
                };
            }



            var user = existingRefreshToken.User;

            if (user == null)
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Errors = new[] { "Refresh Token-ə bağlı istifadəçi tapılmadı." }
                };
            }

            existingRefreshToken.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsync(existingRefreshToken);

            
            var roles = await _userManager.GetRolesAsync(user);
            var newAccessToken = _tokenService.CreateAccessToken(user, roles);
            var newRefreshTokenEntity = _tokenService.CreateRefreshToken(user.Id);

           
            await _refreshTokenRepository.AddAsync(newRefreshTokenEntity);

            
            return new AuthResult
            {
                Succeeded = true,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenEntity.Token
            };
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            
            var existingRefreshToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            
            if (existingRefreshToken == null || existingRefreshToken.IsRevoked)
            {
                
                return true;
            }

            
            existingRefreshToken.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsync(existingRefreshToken);

           
            return true;
        }

        public async Task<bool> IsInRoleAsync(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;
            return await _userManager.IsInRoleAsync(user, roleName);
        }


    }
}
